using Goo.ConfigurationManagement;
using Goo.Helpers;
using System;

namespace Goo.OrmCore
{
    public class DBProviderFactory<TEntity>
    {
        public static DBProviderBase<TEntity> GetDbProvider(OrmConfiguration ormConfiguration)
        {
            try
            {
                Lazy<ConfigurationManagementManager> configReader = new Lazy<ConfigurationManagementManager>(() => new ConfigurationManagementManager());

                Type TE = typeof(TEntity);

                string _providerName = configReader.Value.Get<string>("gooDBProvider");

                string providerName = string.Concat("Goo.OrmCore.", _providerName, "`1[[", TE.AssemblyQualifiedName, "]]");

                Type TP = Type.GetType(providerName, true, true);

                var providerInstance = TP.GetMethod("get_getInstance").Invoke(null, null);

                providerInstance.GetType().GetMethod("SetOrmConfiguration").Invoke(providerInstance, new object[] { ormConfiguration });

                return OrmHelper.getInstance.ConvertTo<DBProviderBase<TEntity>>(providerInstance);
            }
            catch (Exception ex)
            {
                throw new Exception("This provider type is not supported for Goo. Detail:" + ex.ToString());
            }
        }
    }
}