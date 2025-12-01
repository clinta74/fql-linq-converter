using System;
using System.Collections.Generic;

namespace Fql.Linq.Converter.Models;

[Serializable]
public class FilterQuery
{
    public LogicTypes Logic { get; set; }
    public string Field { get; set; } = string.Empty;
    public ICollection<FilterItem> FilterItems { get; set; } = new List<FilterItem>();
}
