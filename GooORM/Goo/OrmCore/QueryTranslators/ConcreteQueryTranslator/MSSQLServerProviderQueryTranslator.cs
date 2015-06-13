using System;
using System.Linq.Expressions;

namespace Goo.OrmCore
{
    public class MSSQLServerProviderQueryTranslator : QueryTranslatorBase
    {
        #region Constuctor
        private static readonly Lazy<MSSQLServerProviderQueryTranslator> instance = new Lazy<MSSQLServerProviderQueryTranslator>(() => new MSSQLServerProviderQueryTranslator());
        public static MSSQLServerProviderQueryTranslator getInstance
        {
            get { return instance.Value; }
        }
        private MSSQLServerProviderQueryTranslator()
        {

        }
        #endregion

        public override string ExpressionQueryToString<T>(Expression<Func<T, object>> predicate)
        {
            string result = new QueryTranslator().Translate(Evaluator.PartialEval(predicate));
            return result;
        }
    }
}