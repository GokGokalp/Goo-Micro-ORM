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
        /// Bu metod kendi kompleks Insert Update  Delete sorgularınızı Stored Procedure , Table Direct , Text olarak oluşturabilmenizi sağlar.
        /// Not Parametre olarak "SqlParameter" array tipi params key ile işaretlendiği için optional olarak argüman geçilmeyebilir.
        /// </summary>
        /// <param name="query">Query statement.</param>
        /// <param name="commandType">Command Object Type.</param>
        /// <param name="sqlParameter">SqlParameters.</param>
        /// <returns>Number of Rows Affected.</returns>
        public int ExecuteCustomNonQuery(string query, System.Data.CommandType commandType, params System.Data.SqlClient.SqlParameter[] sqlParameters)
        {
            var dbManager = new DBManager(OrmConfiguration);
            var cmd = dbManager.CreateCommand(query);

            //We set the command type cmd object
            {
                cmd.CommandType = commandType; // SP // Table Direct // Text
            }

            cmd.Parameters.AddRange(sqlParameters);

            return dbManager.ExecuteNonQuery();
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