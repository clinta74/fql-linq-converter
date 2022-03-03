using System;
using System.Collections.Generic;

namespace FQL.Filters.Linq.Models
{
    [Serializable]
    public class FilterQueryLanguage
    {
        public LogicTypes Logic { get; set; }

        public ICollection<FilterQuery> FilterQueries { get; set; }
    }
}