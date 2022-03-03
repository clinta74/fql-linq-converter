using System.Linq.Expressions;

namespace Fql.Filters.Linq
{
    /// <summary>
    /// Represents a single step in the filter builder.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    internal class FilterBuilderItem<TModel> : FilterBuilderStep<TModel>
        where TModel : class
    {
        private readonly Expression expression;

        /// <summary>
        /// Creates a new step representing the provided expression.
        /// </summary>
        /// <param name="expression">Expression represented by this filter builder step.</param>
        public FilterBuilderItem(Expression expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Gets the expression represented by this filter step.
        /// </summary>
        /// <returns>Expression.</returns>
        public override Expression GetExpression()
        {
            return expression;
        }
    }
}
