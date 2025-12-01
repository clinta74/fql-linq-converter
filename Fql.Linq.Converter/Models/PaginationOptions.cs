namespace Fql.Linq.Converter.Models;

/// <summary>
/// Specifies pagination options for query results.
/// </summary>
public class PaginationOptions
{
    /// <summary>
    /// The page number (1-based). Default is 1.
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// The number of items per page. Default is 20.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
