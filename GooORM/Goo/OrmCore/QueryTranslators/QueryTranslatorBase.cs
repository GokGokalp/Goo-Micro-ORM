using System;
using System.Linq.Expressions;

namespace Goo.OrmCore
{
    public abstract class QueryTranslatorBase
    {
        #region Abstract Methods
        public abstract string ExpressionQueryToString<T>(Expression<Func<T, object>> predicate);
        #endregion    
    }
}