using System;
using System.Collections.Generic;

namespace Fql.Linq.Converter.Models;

[Serializable]
public class FilterQueryLanguage
{
    public LogicTypes Logic { get; set; }

    public ICollection<FilterQuery> FilterQueries { get; set; } = new List<FilterQuery>();

    /// <summary>
    /// Optional sorting instructions. If null or empty, no sorting is applied.
    /// </summary>
    public ICollection<SortDescriptor>? Sorting { get; set; }

    /// <summary>
    /// Optional pagination settings. If null, no pagination is applied.
    /// </summary>
    public PaginationOptions? Pagination { get; set; }
}