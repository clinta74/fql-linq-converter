using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQL.Filters
{
    public enum FilterTypes { text, list, numeric };
    public enum OperationTypes { Eq, NEq, GT, LT, GTE, LTE, Contains, StartsWith, EndsWith}
    public enum LogicTypes { And, Or, Not }
}
