using System;
using System.Linq.Expressions;
using FQL.Filters.Models;

namespace FQL.Filters
{
    public static class FilterHelper
    {
        public static Expression<Func<TModel, bool>> Convert<TModel>(FilterQueryLanguage fql)
            where TModel : class
        {
            var builder = new FilterExpressionBuilder<TModel>(fql.Logic);

            foreach (var filter in fql.FilterQueries)
            {
                bool isGroup = filter.FilterItems.Count > 1;

                if (isGroup)
                {
                    builder.BeginGroup(filter.Logic);
                }

                foreach (var item in filter.FilterItems)
                {
                    Action<string, string, bool> action;

                    switch (item.Operation)
                    {
                        case OperationTypes.Eq:
                            action = builder.BuildEquals;
                            break;

                        case OperationTypes.NEq:
                            action = builder.BuildNotEquals;
                            break;

                        case OperationTypes.GT:
                            action = builder.BuildGreaterThan;
                            break;

                        case OperationTypes.LT:
                            action = builder.BuildLessThan;
                            break;

                        case OperationTypes.GTE:
                            action = builder.BuildGreaterThanOrEquals;
                            break;

                        case OperationTypes.LTE:
                            action = builder.BuildLessThanOrEquals;
                            break;

                        case OperationTypes.Contains:
                            action = builder.BuildContains;
                            break;

                        case OperationTypes.StartsWith:
                            action = builder.BuildStartsWith;
                            break;

                        case OperationTypes.EndsWith:
                            action = builder.BuildEndsWith;
                            break;

                        default:
                            throw new NotImplementedException(String.Format("Operator type {0} has not been implemented.", item.Operation));
                    }

                    action(filter.Field, item.Value, item.IsPreset);
                }

                if (isGroup)
                {
                    builder.EndGroup();
                }
            }

            return builder.GetResult();
        }
    }
}
