using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fql.Filters.Linq
{
    /// <summary>
    /// Represents multiple steps in the filter builder.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    internal class FilterBuilderGroup<TModel> : FilterBuilderStep<TModel>
        where TModel : class
    {
        private readonly FilterBuilderGroup<TModel> parent;
        private readonly List<FilterBuilderStep<TModel>> steps;
        private readonly LogicTypes logicType;

        /// <summary>
        /// Creates a new grouping of filter steps.
        /// </summary>
        /// <param name="logicType">Type of join logic to use.</param>
        /// <param name="parent">Parent of this group.</param>
        public FilterBuilderGroup(LogicTypes logicType, FilterBuilderGroup<TModel> parent)
        {
            if (logicType == LogicTypes.NOT)
            {
                throw new InvalidOperationException("Cannot group filters using 'not' logic.");
            }

            this.logicType = logicType;
            this.parent = parent;
            this.steps = new List<FilterBuilderStep<TModel>>();
        }

        /// <summary>
        /// Gets the parent of this filter group.
        /// </summary>
        public FilterBuilderGroup<TModel> Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Adds a new step to the group.
        /// </summary>
        /// <param name="step">Step to add.</param>
        public void Add(FilterBuilderStep<TModel> step)
        {
            steps.Add(step);
        }

        /// <summary>
        /// Gets the expression represented by this filter step.
        /// </summary>
        /// <returns>Expression.</returns>
        public override Expression GetExpression()
        {
            if (steps.Count == 0)
            {
                return null;
            }

            var joinFn = logicType == LogicTypes.AND ? (Func<Expression, Expression, BinaryExpression>)
                Expression.AndAlso :
                Expression.OrElse;

            return steps.Select(s => s.GetExpression())
                        .Aggregate((acc, next) => joinFn(acc, next));
        }
    }
}
