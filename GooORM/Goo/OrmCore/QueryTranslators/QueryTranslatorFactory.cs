using Goo.Helpers;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Goo.OrmCore
{
    public class QueryTranslatorFactory
    {
        public static QueryTranslatorBase Create(string providerName)
        {
            try
            {
                string _providerName = string.Concat("Goo.OrmCore.", providerName, "QueryTranslator");

                Type TP = Type.GetType(_providerName, true, true);

                var providerInstance = TP.GetMethod("get_getInstance").Invoke(null, null);

                return OrmHelper.getInstance.ConvertTo<QueryTranslatorBase>(providerInstance);
            }
            catch (Exception ex)
            {
                throw new Exception("This provider type is not supported for QuertyTranslator. Detail:" + ex.ToString());
            }
        }
    }
}