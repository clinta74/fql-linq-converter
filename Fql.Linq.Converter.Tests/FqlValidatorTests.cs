using Fql.Linq.Converter;
using Fql.Linq.Converter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Fql.Linq.Converter.Tests;

[TestClass]
public class FqlValidatorTests
{
    [TestMethod]
    public void Test_ValidFql_PassesValidation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Test_NullFql_ReturnsError()
    {
        var result = FqlValidator.Validate<TestModel>(null);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].Contains("cannot be null"));
    }

    [TestMethod]
    public void Test_InvalidFieldName_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "NonExistentField",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("NonExistentField") && e.Contains("does not exist")));
    }

    [TestMethod]
    public void Test_EmptyFieldName_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("empty or null field name")));
    }

    [TestMethod]
    public void Test_ValidNestedProperty_PassesValidation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Address.City",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Seattle" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModelWithNested>(fql);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Test_InvalidNestedProperty_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Address.InvalidProperty",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModelWithNested>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Address.InvalidProperty")));
    }

    [TestMethod]
    public void Test_NotContains_Operation_IsValid()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.NCONTAINS, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid); // NCONTAINS is now implemented
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Test_EmptyFilterValue_ReturnsWarning()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid); // Warnings don't make it invalid
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("empty or null value")));
    }

    [TestMethod]
    public void Test_EmptyFilterQueries_ReturnsWarning()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = Array.Empty<FilterQuery>()
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("FilterQueries is empty")));
    }

    [TestMethod]
    public void Test_InvalidSortField_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            Sorting = new[]
            {
                new SortDescriptor { Field = "InvalidField", Direction = SortDirection.Ascending }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("InvalidField") && e.Contains("does not exist")));
    }

    [TestMethod]
    public void Test_ValidSorting_PassesValidation()
    {
        var fql = new FilterQueryLanguage
        {
            Sorting = new[]
            {
                new SortDescriptor { Field = "Name", Direction = SortDirection.Ascending }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Test_InvalidPagination_PageZero_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            Pagination = new PaginationOptions { Page = 0, PageSize = 20 }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Page number must be >= 1")));
    }

    [TestMethod]
    public void Test_InvalidPagination_PageSizeZero_ReturnsError()
    {
        var fql = new FilterQueryLanguage
        {
            Pagination = new PaginationOptions { Page = 1, PageSize = 0 }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Page size must be >= 1")));
    }

    [TestMethod]
    public void Test_LargePageSize_ReturnsWarning()
    {
        var fql = new FilterQueryLanguage
        {
            Pagination = new PaginationOptions { Page = 1, PageSize = 5000 }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("Page size is very large")));
    }

    [TestMethod]
    public void Test_ValidPagination_PassesValidation()
    {
        var fql = new FilterQueryLanguage
        {
            Pagination = new PaginationOptions { Page = 2, PageSize = 20 }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Test_ComplexFql_MultipleErrors()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "InvalidField1",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                },
                new FilterQuery
                {
                    Field = "InvalidField2",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.NCONTAINS, Value = "Test" }
                    }
                }
            },
            Sorting = new[]
            {
                new SortDescriptor { Field = "InvalidSortField", Direction = SortDirection.Ascending }
            },
            Pagination = new PaginationOptions { Page = 0, PageSize = 0 }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count >= 5); // Multiple errors
    }

    [TestMethod]
    public void Test_FieldNameConverter_AppliedCorrectly()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "name", // lowercase
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test" }
                    }
                }
            }
        };

        // Without converter - should fail
        var resultWithoutConverter = FqlValidator.Validate<TestModel>(fql);
        Assert.IsFalse(resultWithoutConverter.IsValid);

        // With converter - should pass
        var resultWithConverter = FqlValidator.Validate<TestModel>(fql, FilterHelper.ToPascalCase);
        Assert.IsTrue(resultWithConverter.IsValid);
    }

    [TestMethod]
    public void Test_ValidationResult_ToString()
    {
        var result = new ValidationResult();
        result.AddError("Error 1");
        result.AddError("Error 2");
        result.AddWarning("Warning 1");

        var output = result.ToString();

        Assert.IsTrue(output.Contains("Errors (2)"));
        Assert.IsTrue(output.Contains("Error 1"));
        Assert.IsTrue(output.Contains("Error 2"));
        Assert.IsTrue(output.Contains("Warnings (1)"));
        Assert.IsTrue(output.Contains("Warning 1"));
    }

    [TestMethod]
    public void Test_StringOperation_ReturnsWarning()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.CONTAINS, Value = "Test" }
                    }
                }
            }
        };

        var result = FqlValidator.Validate<TestModel>(fql);

        Assert.IsTrue(result.IsValid);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("String operation") && w.Contains("CONTAINS")));
    }
}
