using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FQL.Filters.Linq
{
    /// <summary>
    /// Provides an interface for building a filter expression in a number of build steps.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    public class FilterExpressionBuilder<TModel> : IFilterExpressionBuilder<TModel>
        where TModel : class
    {
        private static readonly Type modelType = typeof(TModel);

        private FilterBuilderGroup<TModel> currentGroup;
        private ParameterExpression parameter;

        public FilterExpressionBuilder(LogicTypes logicType = LogicTypes.AND)
        {
            currentGroup = new FilterBuilderGroup<TModel>(logicType, null);
            parameter = Expression.Parameter(modelType, "model");
        }

        /// <summary>
        /// Gets an expression representing the result of building together filter statements.
        /// </summary>
        /// <returns>Filter expression.</returns>
        public Expression<Func<TModel, bool>> GetResult()
        {
            // implicitly close any open groups
            while (currentGroup.Parent != null)
            {
                EndGroup();
            }

            return Expression.Lambda<Func<TModel, bool>>(currentGroup.GetExpression(), parameter);
        }

        /// <summary>
        /// Builds an equality comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildEquals(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.Equal(l, r));
        }

        /// <summary>
        /// Builds an inequality comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildNotEquals(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.NotEqual(l, r));
        }

        /// <summary>
        /// Builds a greater-than comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildGreaterThan(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.GreaterThan(l, r));
        }

        /// <summary>
        /// Builds a less-than comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildLessThan(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.LessThan(l, r));
        }

        /// <summary>
        /// Builds a greater-than-or-equal-to comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildGreaterThanOrEquals(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.GreaterThanOrEqual(l, r));
        }

        /// <summary>
        /// Builds a less-than-or-equal-to comparison.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildLessThanOrEquals(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => Expression.LessThanOrEqual(l, r));
        }

        /// <summary>
        /// Builds a statement that checks whether the property collection contains a value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildContains(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => GetStringMethod("Contains", l, r));
        }

        /// <summary>
        /// Builds a string comparison that checks if the property starts with the provided value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildStartsWith(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => GetStringMethod("StartsWith", l, r));
        }

        /// <summary>
        /// Builds a string comparison that checks if the property ends with the provided value.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        public void BuildEndsWith(string property, string value, bool valueIsProperty = false)
        {
            Build(property, value, valueIsProperty, (l, r) => GetStringMethod("EndsWith", l, r));
        }

        /// <summary>
        /// Begins a grouping of statements with the provided logic type.
        /// </summary>
        public void BeginGroup(LogicTypes logicType)
        {
            var newGroup = new FilterBuilderGroup<TModel>(logicType, currentGroup);

            currentGroup.Add(newGroup);
            currentGroup = newGroup;
        }

        /// <summary>
        /// Ends the innermost grouping of statements.
        /// </summary>
        public void EndGroup()
        {
            currentGroup = currentGroup.Parent;
        }

        #region Helper Methods

        /// <summary>
        /// Builds an expression using the provided parameters.
        /// </summary>
        /// <param name="property">Name of the model property.</param>
        /// <param name="value">Value to compare to.</param>
        /// <param name="valueIsProperty">Whether or not the value parameter represents a property.</param>
        /// <param name="joinFn">Function used to join the resulting expressions with.</param>
        private void Build(string property, string value, bool valueIsProperty, Func<Expression, Expression, Expression> joinFn)
        {
            if (String.IsNullOrWhiteSpace(property)) throw new ArgumentNullException("property");
            if (value == null) throw new ArgumentNullException("value");

            var left = GetPropertyExpression(property);
            var propType = (left as MemberExpression).Type;
            var right = valueIsProperty ? GetPropertyExpression(value) : Expression.Constant(Convert.ChangeType(value, propType), propType);

            var expression = joinFn(left, right);

            currentGroup.Add(new FilterBuilderItem<TModel>(expression));
        }

        /// <summary>
        /// Returns an expression representing the named property.
        /// </summary>
        /// <param name="property">Property name/path.</param>
        /// <returns>Expression representing the property to access.</returns>
        private Expression GetPropertyExpression(string property)
        {
            string[] properties = property.Split('.');

            try
            {
            return properties.Aggregate(new Tuple<Expression, Type>(parameter, parameter.Type), (acc, next) =>
                {
                    var expression = Expression.MakeMemberAccess(acc.Item1, acc.Item2.GetProperty(next));

                    return new Tuple<Expression, Type>(expression, expression.Type);
                }).Item1;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException($"Property {property} could not be found in object {parameter.Type.ToString()}");
            }
        }

        /// <summary>
        /// Gets a string method to call.
        /// </summary>
        /// <param name="methodName">String method name.</param>
        /// <param name="instance">String instance.</param>
        /// <param name="arguments">Arguments to supply to the method.</param>
        /// <returns>Method call expression.</returns>
        private Expression GetStringMethod(string methodName, Expression instance, params Expression[] arguments)
        {
            Type[] typeArgs = arguments.Select(a => typeof(string)).ToArray();
            MethodInfo method = typeof(string).GetMethod(methodName, typeArgs);

            if (method == null)
            {
                throw new InvalidOperationException(String.Format("Could not find method '{0}' on type string.", methodName));
            }

            return Expression.Call(instance, method, arguments);
        }

        #endregion
    }
}
