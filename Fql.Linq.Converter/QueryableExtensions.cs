using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fql.Linq.Converter.Models;

namespace Fql.Linq.Converter;

/// <summary>
/// Extension methods for applying FQL (Filter Query Language) to IQueryable.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies filtering, sorting, and pagination from FQL to an IQueryable.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="query">The query to apply FQL to.</param>
    /// <param name="fql">The Filter Query Language object.</param>
    /// <param name="fieldNameConverter">Optional function to convert field names (e.g., camelCase to PascalCase).</param>
    /// <returns>The modified query with filters, sorting, and pagination applied.</returns>
    public static IQueryable<TModel> ApplyFql<TModel>(
        this IQueryable<TModel> query,
        FilterQueryLanguage fql,
        Func<string, string>? fieldNameConverter = null)
        where TModel : class
    {
        if (fql == null)
            return query;

        fieldNameConverter ??= (value) => value;

        // Apply filtering
        if (fql.FilterQueries?.Any() == true)
        {
            var filterExpression = FilterHelper.Convert<TModel>(fql, fieldNameConverter);
            query = query.Where(filterExpression);
        }

        // Apply sorting
        if (fql.Sorting?.Any() == true)
        {
            query = ApplySorting(query, fql.Sorting, fieldNameConverter);
        }

        // Apply pagination
        if (fql.Pagination != null)
        {
            query = ApplyPagination(query, fql.Pagination);
        }

        return query;
    }

    /// <summary>
    /// Applies sorting to an IQueryable based on sort descriptors.
    /// </summary>
    private static IQueryable<TModel> ApplySorting<TModel>(
        IQueryable<TModel> query,
        ICollection<SortDescriptor> sorting,
        Func<string, string> fieldNameConverter)
        where TModel : class
    {
        if (sorting == null || !sorting.Any())
            return query;

        IOrderedQueryable<TModel>? orderedQuery = null;

        foreach (var sort in sorting)
        {
            var field = fieldNameConverter(sort.Field);
            var parameter = Expression.Parameter(typeof(TModel), "model");
            var property = GetPropertyExpression(parameter, field);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = orderedQuery == null
                ? (sort.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending")
                : (sort.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending");

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TModel), property.Type);

            orderedQuery = (IOrderedQueryable<TModel>)method.Invoke(null, new object[] { orderedQuery ?? query, lambda })!;
        }

        return orderedQuery ?? query;
    }

    /// <summary>
    /// Applies pagination to an IQueryable.
    /// </summary>
    private static IQueryable<TModel> ApplyPagination<TModel>(
        IQueryable<TModel> query,
        PaginationOptions pagination)
    {
        if (pagination.Page < 1)
            pagination.Page = 1;

        if (pagination.PageSize < 1)
            pagination.PageSize = 20;

        var skip = (pagination.Page - 1) * pagination.PageSize;
        return query.Skip(skip).Take(pagination.PageSize);
    }

    /// <summary>
    /// Gets a property expression for a potentially nested property path.
    /// </summary>
    private static Expression GetPropertyExpression(Expression parameter, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        Expression expression = parameter;
        Type currentType = parameter.Type;

        foreach (var propertyName in properties)
        {
            var propertyInfo = currentType.GetProperty(propertyName)
                ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{currentType.Name}'");

            expression = Expression.Property(expression, propertyInfo);
            currentType = propertyInfo.PropertyType;
        }

        return expression;
    }
}
