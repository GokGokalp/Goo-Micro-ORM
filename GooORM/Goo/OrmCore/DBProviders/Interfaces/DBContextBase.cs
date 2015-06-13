using System;
using System.Data.Common;

namespace Goo.OrmCore
{
    public abstract class DBContextBase
    {
        #region Constructor
        protected DBContextBase()
        {

        }
        #endregion

        #region Properties
        private OrmConfiguration _ormConfiguration = new OrmConfiguration();
        public OrmConfiguration OrmConfiguration
        {
            get
            {
                return _ormConfiguration;
            }
            set { _ormConfiguration = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Bu metot kendi kompleks sorgunuzu oluşturabilmenizi sağlar.
        /// This method can be used to perform complex queries.
        /// </summary>
        /// <param name="query">Query statement.</param>
        /// <param name="parameters">Query parameters. This is not a required.</param>
        /// <returns>This method will return DbDataReader object.</returns>
        public DbDataReader ExecuteCustomQuery(string query, object parameters = null)
        {
            var dbManager = new DBManager(OrmConfiguration);

            dbManager.CreateCommand(query);

            if (parameters != null)
                dbManager.AddParameter(parameters);

            var reader = dbManager.ExecuteReader();

            return reader;
        }

        /// <summary>
        /// Ekleme, silme veya güncelleme gibi yapılmış olan işlemleri, veritabanında uygulamayı sağlar.
        /// Allows you to perform insert, delete or update operations on the database.
        /// </summary>
        /// <returns></returns>
        public int SubmitChanges()
        {
            var dbManager = new DBManager(OrmConfiguration);

            dbManager.CreateCommand(DBManager.CommandText);

            if (DBManager.CommandParameters != null && DBManager.CommandParameters.Count > 0)
                foreach (var loopParameter in DBManager.CommandParameters)
                {
                    dbManager.AddParameter(loopParameter.Key.ToString(), loopParameter.Value);
                }

            int status = dbManager.ExecuteNonQuery();

            return status;
        }
        #endregion
    }
}