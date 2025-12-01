namespace Fql.Linq.Converter.Models;

/// <summary>
/// Describes how a field should be sorted.
/// </summary>
public class SortDescriptor
{
    /// <summary>
    /// The field name to sort by. Supports nested properties (e.g., "User.Name").
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// The direction to sort (Ascending or Descending).
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}
