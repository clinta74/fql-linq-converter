using System.Linq.Expressions;

namespace Fql.Linq.Converter;

/// <summary>
/// Represents a step in the filter builder.
/// </summary>
/// <typeparam name="TModel">Model type.</typeparam>
internal abstract class FilterBuilderStep<TModel>
    where TModel : class
{
    /// <summary>
    /// Gets the expression represented by this filter step.
    /// </summary>
    /// <returns>Expression.</returns>
    public abstract Expression GetExpression();
}
