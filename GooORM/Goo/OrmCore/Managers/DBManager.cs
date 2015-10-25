using Goo.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;

namespace Goo.OrmCore
{
    public class DBManager : Disposable
    {
        #region Constructor
        public DBManager(OrmConfiguration ormConfiguration)
        {
            _ormConfiguration = ormConfiguration;
        }
        #endregion

        #region Properties
        public static string CommandText;
        public static Dictionary<object, dynamic> CommandParameters;
        #endregion

        #region Private Variables
        private DbCommand command = null;

        private static Lazy<DbProviderFactory> provider = new Lazy<DbProviderFactory>(() => DbProviderFactories.GetFactory(OrmHelper.getInstance.DbProviderInvariantName));

        private OrmConfiguration _ormConfiguration;
        #endregion

        #region Private Methods
        public void OpenConnection()
        {
            if (_ormConfiguration.Connection.State != System.Data.ConnectionState.Open)
            {
                _ormConfiguration.Connection.Open();
            }
        }

        private void CloseConnection()
        {
            _ormConfiguration.Connection.Close();
        }
        #endregion

        #region Public Methods
        public DbCommand CreateCommand(string commandText)
        {
            command = _ormConfiguration.Connection.CreateCommand();
            command.CommandText = commandText;
            command.Connection = _ormConfiguration.Connection;
            command.Transaction = _ormConfiguration.Transaction;

            return command;
        }

        public DbCommand CreateCommand(string commandText, System.Data.CommandType commandType)
        {
            command = _ormConfiguration.Connection.CreateCommand();
            command.CommandText = commandText;
            command.Connection = _ormConfiguration.Connection;
            command.Transaction = _ormConfiguration.Transaction;
            command.CommandType = commandType;

            return command;
        }

        public void AddParameter<T>(string name, T value)
        {
            if (command != null)
            {
                var parameter = provider.Value.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }
            else
                throw new Exception("DbCommand object can not be null.");
        }

        public void AddParameter(object parameters)
        {
            if (command != null && parameters != null)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(parameters);
                foreach (PropertyDescriptor loopParam in props)
                {
                    var paramName = loopParam.Name;
                    var paramValue = loopParam.GetValue(parameters);

                    AddParameter<object>(paramName, paramValue);
                }
            }
            else
                throw new Exception("DbCommand or parameters object can not be null.");
        }

        public void AddParameter(System.Data.SqlClient.SqlParameter[] sqlParams)
        {
            if (command != null)
                command.Parameters.AddRange(sqlParams);
        }

        public DbDataReader ExecuteReader()
        {
            OpenConnection();

            return command.ExecuteReader();
        }

        public int ExecuteNonQuery()
        {
            OpenConnection();

            return command.ExecuteNonQuery();
        }
        #endregion

        #region Disposable Members
        protected override void DisposeCore()
        {
            if (_ormConfiguration.Connection != null)
            {
                CloseConnection();
            }

            if (command != null)
            {
                command.Dispose();
            }

            CommandText = null;
            CommandParameters = null;
        }
        #endregion
    }
}