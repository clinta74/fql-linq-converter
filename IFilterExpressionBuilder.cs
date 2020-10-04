using System;
using System.Linq.Expressions;

namespace FQL.Filters.Linq
{
    /// <summary>
    /// Provides required methods for implementing a filtering expression builder.
    /// </summary>
    /// <typeparam name="TModel">Type of model to filter on.</typeparam>
    public interface IFilterExpressionBuilder<TModel>
        where TModel : class
    {
        /// <summary>
        /// Gets an expression representing the result of building together filter statements.
        /// </summary>
        /// <returns>Filter expression.</returns>
        Expression<Func<TModel, bool>> GetResult();

        /// <summary>
        /// Builds an equality comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildEquals(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds an inequality comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildNotEquals(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a greater-than comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildGreaterThan(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a less-than comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildLessThan(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a greater-than-or-equal-to comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildGreaterThanOrEquals(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a less-than-or-equal-to comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildLessThanOrEquals(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a statement that checks whether the property collection contains a value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildContains(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a string comparison that checks if the property starts with the provided value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildStartsWith(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Builds a string comparison that checks if the property ends with the provided value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        void BuildEndsWith(string property, string value, bool valueIsProperty = false);

        /// <summary>
        /// Begins a grouping of statements with the provided logic type.
        /// </summary>
        void BeginGroup(LogicTypes logicType);

        /// <summary>
        /// Ends the innermost grouping of statements.
        /// </summary>
        void EndGroup();
    }
}
