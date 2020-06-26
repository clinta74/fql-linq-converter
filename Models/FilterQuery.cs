using System;
using System.Collections.Generic;

namespace FQL.Filters.Models
{
    [Serializable]
    public class FilterQuery
    {
        public LogicTypes Logic { get; set; }
        public string Field { get; set; }
        public ICollection<FilterItem> FilterItems { get; set; }
    }
}
