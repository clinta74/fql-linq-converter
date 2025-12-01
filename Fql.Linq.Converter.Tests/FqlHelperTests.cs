using Fql.Linq.Converter;
using Fql.Linq.Converter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Fql.Linq.Converter.Tests;

[TestClass]
public class FqlHelperTests
{
    [TestMethod]
    public void TestCovert()
    {
        // arrange
        FilterQueryLanguage fql = new FilterQueryLanguage
        {
            FilterQueries = new[] 
            { 
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem
                        {
                            Operation = OperationTypes.EQ,
                            Value = "Test",
                            IsPreset = false,
                        }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        // act

        var exp = FilterHelper.Convert<TestModel>(fql);

        // assert

        Assert.AreEqual("(model.Name == \"Test\")", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_Field_Case_NotChanged()
    {
        // arrange
        FilterQueryLanguage fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "name", // camel case field
                    FilterItems = new[]
                    {
                        new FilterItem
                        {
                            Operation = OperationTypes.EQ,
                            Value = "Test",
                            IsPreset = false,
                        }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        // act

        Assert.ThrowsException<ArgumentException>(() => FilterHelper.Convert<TestModel>(fql));
    }

    [TestMethod]
    public void Test_WithPropertyCaseFix()
    {
        // arrange
        FilterQueryLanguage fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "name", // camel case field
                    FilterItems = new[]
                    {
                        new FilterItem
                        {
                            Operation = OperationTypes.EQ,
                            Value = "Test",
                            IsPreset = false,
                        }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        // act

        var exp = FilterHelper.Convert<TestModel>(fql, (value) => FilterHelper.ToPascalCase(value));

        // assert

        Assert.AreEqual("(model.Name == \"Test\")", exp.Body.ToString());
    }

    #region Operation Type Tests

    [TestMethod]
    public void Test_NotEquals_Operation()
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
                        new FilterItem { Operation = OperationTypes.NEQ, Value = "Test" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.AreEqual("(model.Name != \"Test\")", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_GreaterThan_Operation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.GT, Value = "5" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.AreEqual("(model.Id > 5)", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_LessThan_Operation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.LT, Value = "10" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.AreEqual("(model.Id < 10)", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_GreaterThanOrEqual_Operation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.GTE, Value = "5" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.AreEqual("(model.Id >= 5)", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_LessThanOrEqual_Operation()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.LTE, Value = "10" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.AreEqual("(model.Id <= 10)", exp.Body.ToString());
    }

    [TestMethod]
    public void Test_Contains_Operation()
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
                        new FilterItem { Operation = OperationTypes.CONTAINS, Value = "test" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.IsTrue(exp.Body.ToString().Contains("Contains"));
        Assert.IsTrue(exp.Body.ToString().Contains("test"));
    }

    [TestMethod]
    public void Test_StartsWith_Operation()
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
                        new FilterItem { Operation = OperationTypes.STARTS, Value = "Test" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.IsTrue(exp.Body.ToString().Contains("StartsWith"));
        Assert.IsTrue(exp.Body.ToString().Contains("Test"));
    }

    [TestMethod]
    public void Test_EndsWith_Operation()
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
                        new FilterItem { Operation = OperationTypes.ENDS, Value = "Test" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);

        Assert.IsTrue(exp.Body.ToString().Contains("EndsWith"));
        Assert.IsTrue(exp.Body.ToString().Contains("Test"));
    }

    #endregion

    #region Logic Type Tests

    [TestMethod]
    public void Test_AND_Logic_MultipleFilters()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GT, Value = "5" } }
                },
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "Test" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("AndAlso"));
        Assert.IsTrue(body.Contains("model.Id > 5"));
        Assert.IsTrue(body.Contains("model.Name == \"Test\""));
    }

    [TestMethod]
    public void Test_OR_Logic_MultipleFilters()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "1" } }
                },
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "2" } }
                }
            },
            Logic = LogicTypes.OR
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("OrElse"));
    }

    [TestMethod]
    public void Test_MultipleFilterItems_InSameQuery()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test1" },
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test2" }
                    }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("OrElse"));
        Assert.IsTrue(body.Contains("Test1"));
        Assert.IsTrue(body.Contains("Test2"));
    }

    [TestMethod]
    public void Test_ComplexNested_ANDandOR()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test1" },
                        new FilterItem { Operation = OperationTypes.EQ, Value = "Test2" }
                    }
                },
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GT, Value = "5" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("AndAlso"));
        Assert.IsTrue(body.Contains("OrElse"));
    }

    #endregion

    #region Data Type Tests

    [TestMethod]
    public void Test_IntegerProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "42" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var compiled = exp.Compile();

        Assert.IsTrue(compiled(new TestModel(42, "Test")));
        Assert.IsFalse(compiled(new TestModel(1, "Test")));
    }

    [TestMethod]
    public void Test_BooleanProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "IsActive",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "true" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithTypes>(fql);
        var compiled = exp.Compile();

        Assert.IsTrue(compiled(new TestModelWithTypes { IsActive = true }));
        Assert.IsFalse(compiled(new TestModelWithTypes { IsActive = false }));
    }

    [TestMethod]
    public void Test_DateTimeProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "CreatedDate",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GT, Value = "2024-01-01" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithTypes>(fql);
        var compiled = exp.Compile();

        Assert.IsTrue(compiled(new TestModelWithTypes { CreatedDate = new DateTime(2024, 6, 1) }));
        Assert.IsFalse(compiled(new TestModelWithTypes { CreatedDate = new DateTime(2023, 1, 1) }));
    }

    [TestMethod]
    public void Test_DecimalProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Price",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GTE, Value = "10.50" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithTypes>(fql);
        var compiled = exp.Compile();

        Assert.IsTrue(compiled(new TestModelWithTypes { Price = 10.50m }));
        Assert.IsTrue(compiled(new TestModelWithTypes { Price = 15.00m }));
        Assert.IsFalse(compiled(new TestModelWithTypes { Price = 9.99m }));
    }

    [TestMethod]
    public void Test_EnumProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Status",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "Active" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithTypes>(fql);
        var compiled = exp.Compile();

        Assert.IsTrue(compiled(new TestModelWithTypes { Status = TestStatus.Active }));
        Assert.IsFalse(compiled(new TestModelWithTypes { Status = TestStatus.Inactive }));
    }

    #endregion

    #region Nested Property Tests

    [TestMethod]
    public void Test_NestedProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Address.City",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "Seattle" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithNested>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("Address.City"));
    }

    [TestMethod]
    public void Test_DeepNestedProperty()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Address.Location.Latitude",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GT, Value = "0" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModelWithNested>(fql);
        var body = exp.Body.ToString();

        Assert.IsTrue(body.Contains("Address.Location.Latitude"));
    }

    #endregion

    #region Error Handling Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_InvalidProperty_ThrowsException()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "NonExistentProperty",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "Test" } }
                }
            },
            Logic = LogicTypes.AND
        };

        FilterHelper.Convert<TestModel>(fql);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_InvalidNestedProperty_ThrowsException()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Address.InvalidProperty",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.EQ, Value = "Test" } }
                }
            },
            Logic = LogicTypes.AND
        };

        FilterHelper.Convert<TestModelWithNested>(fql);
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public void Test_UnimplementedOperation_ThrowsException()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Field = "Name",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.NCONTAINS, Value = "Test" } }
                }
            },
            Logic = LogicTypes.AND
        };

        FilterHelper.Convert<TestModel>(fql);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void Test_RealWorldScenario_UserFiltering()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = new[]
            {
                new FilterQuery
                {
                    Logic = LogicTypes.OR,
                    Field = "Name",
                    FilterItems = new[]
                    {
                        new FilterItem { Operation = OperationTypes.CONTAINS, Value = "John" },
                        new FilterItem { Operation = OperationTypes.CONTAINS, Value = "Jane" }
                    }
                },
                new FilterQuery
                {
                    Field = "Id",
                    FilterItems = new[] { new FilterItem { Operation = OperationTypes.GT, Value = "100" } }
                }
            },
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var testData = new[]
        {
            new TestModel(101, "John Doe"),
            new TestModel(102, "Jane Smith"),
            new TestModel(99, "John Williams"),
            new TestModel(105, "Bob Smith")
        };

        var results = testData.AsQueryable().Where(exp).ToList();

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(x => x.Name == "John Doe"));
        Assert.IsTrue(results.Any(x => x.Name == "Jane Smith"));
    }

    [TestMethod]
    public void Test_EmptyFilterQueries_ReturnsAllRecords()
    {
        var fql = new FilterQueryLanguage
        {
            FilterQueries = Array.Empty<FilterQuery>(),
            Logic = LogicTypes.AND
        };

        var exp = FilterHelper.Convert<TestModel>(fql);
        var testData = new[]
        {
            new TestModel(1, "Test1"),
            new TestModel(2, "Test2")
        };

        var results = testData.AsQueryable().Where(exp).ToList();

        Assert.AreEqual(2, results.Count);
    }

    #endregion
}

public record TestModel(int Id, string Name);

public class TestModelWithTypes
{
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal Price { get; set; }
    public TestStatus Status { get; set; }
}

public enum TestStatus
{
    Active,
    Inactive,
    Pending
}

public class TestModelWithNested
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
}

public class Address
{
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public Location Location { get; set; } = new Location();
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}