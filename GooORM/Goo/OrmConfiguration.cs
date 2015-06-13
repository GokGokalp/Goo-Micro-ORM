using Goo.Helpers;
using System;
using System.Configuration;
using System.Data.Common;

namespace Goo
{
    public class OrmConfiguration
    {
        #region Private Members
        private static Lazy<DbProviderFactory> provider = new Lazy<DbProviderFactory>(() => DbProviderFactories.GetFactory(OrmHelper.getInstance.DbProviderInvariantName));

        private Lazy<DbConnection> connection = new Lazy<DbConnection>(() =>
        {
            var connection = provider.Value.CreateConnection();

            connection.ConnectionString = ConfigurationManager.ConnectionStrings["gooConnectionString"].ConnectionString;

            return connection;
        });
        #endregion

        #region Properties
        /// <summary>
        /// Lazy loading etkinleştirildiğinde, ilişkilendirilen nesneler navigation bir property üzerinden erişilmeye çalışıldığında yüklenecektir. (varsayılan false)
        /// Lazy loading is enabled, related objects are loaded when they are accessed through a navigation property. (false as default)
        /// </summary>
        public bool LazyLoadingEnabled = false;
        public DbConnection Connection { get { return connection.Value; } }
        public DbTransaction Transaction { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Bu metot transaction kullanabilmenizi sağlar.
        /// With this method, you can use transaction in your entity.
        /// </summary>
        /// <param name="transaction">Transaction object.</param>
        public void UseTransaction(DbTransaction transaction)
        {
            Transaction = transaction;
        }
        #endregion
    }
}