using FQL.Filters.Linq;
using FQL.Filters.Linq.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FQL.Filter.Linq.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestExpression()
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
    }

    public record TestModel(int Id, string Name);
}