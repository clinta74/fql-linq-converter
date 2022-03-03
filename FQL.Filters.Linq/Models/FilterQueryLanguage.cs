using System;
using System.Collections.Generic;

namespace Fql.Filters.Linq.Models
{
    [Serializable]
    public class FilterQueryLanguage
    {
        public LogicTypes Logic { get; set; }

        public ICollection<FilterQuery> FilterQueries { get; set; }
    }
}