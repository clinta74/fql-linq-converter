using System;
using System.Collections.Generic;
using System.Linq;
using Fql.Linq.Converter.Models;

namespace Fql.Linq.Converter;

/// <summary>
/// Provides validation for Filter Query Language (FQL) objects.
/// </summary>
public static class FqlValidator
{
    /// <summary>
    /// Validates an FQL object against a specific model type.
    /// </summary>
    /// <typeparam name="TModel">The model type to validate against.</typeparam>
    /// <param name="fql">The FQL object to validate.</param>
    /// <param name="fieldNameConverter">Optional function to convert field names (e.g., camelCase to PascalCase).</param>
    /// <returns>A ValidationResult containing any errors or warnings.</returns>
    public static ValidationResult Validate<TModel>(
        FilterQueryLanguage? fql,
        Func<string, string>? fieldNameConverter = null)
        where TModel : class
    {
        var result = new ValidationResult();
        fieldNameConverter ??= (value) => value;

        if (fql == null)
        {
            result.AddError("FQL object cannot be null.");
            return result;
        }

        // Validate filter queries
        if (fql.FilterQueries == null)
        {
            result.AddWarning("FilterQueries is null. No filtering will be applied.");
        }
        else if (!fql.FilterQueries.Any())
        {
            result.AddWarning("FilterQueries is empty. No filtering will be applied.");
        }
        else
        {
            ValidateFilterQueries<TModel>(fql.FilterQueries, result, fieldNameConverter);
        }

        // Validate sorting
        if (fql.Sorting != null && fql.Sorting.Any())
        {
            ValidateSorting<TModel>(fql.Sorting, result, fieldNameConverter);
        }

        // Validate pagination
        if (fql.Pagination != null)
        {
            ValidatePagination(fql.Pagination, result);
        }

        return result;
    }

    private static void ValidateFilterQueries<TModel>(
        ICollection<FilterQuery> filterQueries,
        ValidationResult result,
        Func<string, string> fieldNameConverter)
        where TModel : class
    {
        var modelType = typeof(TModel);

        foreach (var query in filterQueries)
        {
            if (string.IsNullOrWhiteSpace(query.Field))
            {
                result.AddError("Filter query has empty or null field name.");
                continue;
            }

            var convertedField = fieldNameConverter(query.Field);
            if (!IsValidProperty<TModel>(convertedField))
            {
                result.AddError($"Field '{query.Field}' (converted to '{convertedField}') does not exist on type '{modelType.Name}'.");
            }

            if (query.FilterItems == null || !query.FilterItems.Any())
            {
                result.AddWarning($"Filter query for field '{query.Field}' has no filter items.");
                continue;
            }

            foreach (var item in query.FilterItems)
            {
                ValidateFilterItem(item, query.Field, result);
            }
        }
    }

    private static void ValidateFilterItem(
        FilterItem item,
        string fieldName,
        ValidationResult result)
    {
        // Check if operation is valid
        if (!Enum.IsDefined(typeof(OperationTypes), item.Operation))
        {
            result.AddError($"Invalid operation type '{item.Operation}' for field '{fieldName}'.");
        }

        // Validate value
        if (string.IsNullOrEmpty(item.Value))
        {
            result.AddWarning($"Filter item for field '{fieldName}' has empty or null value.");
        }

        // Check string operations on appropriate types
        if (item.Operation == OperationTypes.CONTAINS ||
            item.Operation == OperationTypes.NCONTAINS ||
            item.Operation == OperationTypes.STARTS ||
            item.Operation == OperationTypes.ENDS)
        {
            // Note: We can't check the actual type here without more context,
            // but we can warn about potential issues
            result.AddWarning($"String operation '{item.Operation}' used on field '{fieldName}'. Ensure this field is of type string.");
        }
    }

    private static void ValidateSorting<TModel>(
        ICollection<SortDescriptor> sorting,
        ValidationResult result,
        Func<string, string> fieldNameConverter)
        where TModel : class
    {
        var modelType = typeof(TModel);

        foreach (var sort in sorting)
        {
            if (string.IsNullOrWhiteSpace(sort.Field))
            {
                result.AddError("Sort descriptor has empty or null field name.");
                continue;
            }

            var convertedField = fieldNameConverter(sort.Field);
            if (!IsValidProperty<TModel>(convertedField))
            {
                result.AddError($"Sort field '{sort.Field}' (converted to '{convertedField}') does not exist on type '{modelType.Name}'.");
            }

            if (!Enum.IsDefined(typeof(SortDirection), sort.Direction))
            {
                result.AddError($"Invalid sort direction '{sort.Direction}' for field '{sort.Field}'.");
            }
        }
    }

    private static void ValidatePagination(
        PaginationOptions pagination,
        ValidationResult result)
    {
        if (pagination.Page < 1)
        {
            result.AddError($"Page number must be >= 1. Current value: {pagination.Page}");
        }

        if (pagination.PageSize < 1)
        {
            result.AddError($"Page size must be >= 1. Current value: {pagination.PageSize}");
        }

        if (pagination.PageSize > 1000)
        {
            result.AddWarning($"Page size is very large ({pagination.PageSize}). Consider using a smaller value for better performance.");
        }
    }

    /// <summary>
    /// Checks if a property path exists on a type (supports nested properties).
    /// </summary>
    private static bool IsValidProperty<TModel>(string propertyPath)
    {
        var properties = propertyPath.Split('.');
        var currentType = typeof(TModel);

        foreach (var propertyName in properties)
        {
            var propertyInfo = currentType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return false;
            }
            currentType = propertyInfo.PropertyType;
        }

        return true;
    }
}
