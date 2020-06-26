using System;
using System.Collections.Generic;

namespace FQL.Filters.Models
{
    [Serializable]
    public class FilterQueryLanguage
    {
        public LogicTypes Logic { get; set; }

        public IEnumerable<FilterQuery> FilterQueries { get; set; }
    }
}