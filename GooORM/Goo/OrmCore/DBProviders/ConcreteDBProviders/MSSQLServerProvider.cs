using Goo.Attributes;
using Goo.CacheManagement;
using Goo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Goo.OrmCore
{
    public class MSSQLServerProvider<TEntity> : DBProviderBase<TEntity>, IDBTableCreationable where TEntity : class, new()
    {
        #region Private Members
        private OrmConfiguration _ormConfiguration;
        private List<string> primitiveTypes = new List<string> { "Boolean", "Byte", "DateTime", "Decimal", "Double", "Float", "Guid", "Int16", "Int32", "Int64", "SByte", "String", "Time", "Binary" };
        #endregion

        #region Constructor
        private static readonly Lazy<MSSQLServerProvider<TEntity>> instance = new Lazy<MSSQLServerProvider<TEntity>>(() => new MSSQLServerProvider<TEntity>());
        public static MSSQLServerProvider<TEntity> getInstance
        {
            get
            {
                return instance.Value;
            }
        }
        private MSSQLServerProvider()
        {

        }
        #endregion

        #region DBProviderBase Members
        public override DBProviderBase<TEntity> Where(Expression<Func<TEntity, object>> predicate)
        {
            string whereClause = QueryTranslatorFactory.Create("MSSQLServerProvider").ExpressionQueryToString<TEntity>(predicate);

            if (string.IsNullOrEmpty(WhereClause))
                WhereClause += whereClause;
            else
                WhereClause += string.Format(" AND {0} ", whereClause);

            return this;
        }

        public override List<TEntity> GetFromCache(object cacheKey)
        {
            if (CacheManager.getInstance.Contains(cacheKey))
                return OrmHelper.getInstance.ConvertTo<List<TEntity>>(CacheManager.getInstance.Get(cacheKey));

            return null;
        }

        internal override TEntity GetRelation(string foreignColumnName, object value)
        {
            if (_ormConfiguration.LazyLoadingEnabled)
            {
                StringBuilder sbWhereClause = new StringBuilder();

                sbWhereClause.Append(string.Format("{0} = ", foreignColumnName));

                if (value is string)
                {
                    sbWhereClause.Append(string.Format("'{0}'", value));
                }
                else
                {
                    sbWhereClause.Append(value);
                }

                WhereClause = sbWhereClause.ToString();

                var entity = ExecuteSelectQuery().FirstOrDefault();

                return entity;
            }
            else
            {
                return default(TEntity);
            }
        }

        internal override List<TEntity> GetRelations(string foreignColumnName, object value)
        {
            if (_ormConfiguration.LazyLoadingEnabled)
            {
                StringBuilder sbWhereClause = new StringBuilder();

                sbWhereClause.Append(string.Format("{0} = ", foreignColumnName));

                if (value is string)
                {
                    sbWhereClause.Append(string.Format("'{0}'", value));
                }
                else
                {
                    sbWhereClause.Append(value);
                }

                WhereClause = sbWhereClause.ToString();

                var entities = ExecuteSelectQuery().ToList();

                return entities;
            }
            else
            {
                return new List<TEntity>();
            }
        }

        public override void Insert(TEntity entity)
        {
            #region Variables
            string insertQuery = string.Empty;
            var columnNames = new List<string>();
            var columnValues = new Dictionary<object, dynamic>();
            object valueType = new object();
            DBManager dbManager = new DBManager(_ormConfiguration);
            #endregion

            foreach (var loopProperty in EntityType.GetProperties())
            {
                // Check primary key property
                valueType = loopProperty.GetCustomAttributes(typeof(IsPrimaryKey), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                // Check auto increment  property
                valueType = loopProperty.GetCustomAttributes(typeof(IsAutoIncrement), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                // Add parameters
                if (primitiveTypes.Contains(loopProperty.PropertyType.Name))
                {
                    columnNames.Add(string.Format("[{0}]", loopProperty.Name));
                    columnValues.Add(loopProperty.Name, loopProperty.GetValue(entity));
                }
            }

            insertQuery = string.Format("INSERT INTO [dbo].[{0}] ({1}) VALUES ({2})", EntityType.Name, string.Join(",", columnNames), string.Join(",@", columnNames).Replace("[", "").Replace("]", "")).Replace("VALUES (", "VALUES (@");

            DBManager.CommandText = insertQuery;
            DBManager.CommandParameters = columnValues;
        }

        public override void Update(TEntity entity)
        {
            #region Variables
            string updateQuery = string.Empty;
            StringBuilder setQuery = new StringBuilder();
            object valueType = new object();
            object primaryKeyColumnName = new object();
            object primaryKeyColumnValue = new object();
            DBManager dbManager = new DBManager(_ormConfiguration);
            #endregion

            foreach (var loopProperty in EntityType.GetProperties())
            {
                // Check primary key property
                valueType = loopProperty.GetCustomAttributes(typeof(IsPrimaryKey), false).FirstOrDefault();
                if (valueType != null)
                {
                    primaryKeyColumnName = loopProperty.Name;
                    primaryKeyColumnValue = loopProperty.GetValue(entity);

                    continue;
                }

                // Check auto increment property
                valueType = loopProperty.GetCustomAttributes(typeof(IsAutoIncrement), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                // Add parameters
                if (primitiveTypes.Contains(loopProperty.PropertyType.Name))
                {
                    setQuery.Append(string.Format(" [{0}] =", loopProperty.Name));

                    valueType = loopProperty.GetValue(entity);

                    if (valueType is string)
                    {
                        setQuery.Append(string.Format(" '{0}', ", valueType));
                    }
                    else
                        setQuery.Append(string.Format(" {0}, ", valueType));
                }
            }

            updateQuery = string.Format("UPDATE [dbo].[{0}] SET {1} WHERE [{2}] = {3}", EntityType.Name, setQuery.ToString().Replace("True", "1").Replace("False", "0").Trim().TrimEnd(','), primaryKeyColumnName.ToString(), primaryKeyColumnValue.ToString());

            DBManager.CommandText = updateQuery;
        }

        public override void Delete(TEntity entity)
        {
            #region Variables
            string deleteQuery = string.Empty;
            object valueType = new object();
            object primaryKeyColumnName = new object();
            object primaryKeyColumnValue = new object();
            DBManager dbManager = new DBManager(_ormConfiguration);
            #endregion

            foreach (var loopProperty in EntityType.GetProperties())
            {
                // Check primary key property
                valueType = loopProperty.GetCustomAttributes(typeof(IsPrimaryKey), false).FirstOrDefault();
                if (valueType != null)
                {
                    primaryKeyColumnName = loopProperty.Name;
                    primaryKeyColumnValue = loopProperty.GetValue(entity);

                    break;
                }
            }

            deleteQuery = string.Format("DELETE FROM [dbo].[{0}] WHERE [{1}] = {2}", EntityType.Name, primaryKeyColumnName.ToString(), primaryKeyColumnValue.ToString());

            DBManager.CommandText = deleteQuery;
        }

        #region Fluent Methods
        public override DBProviderBase<TEntity> OrderByDescending(Expression<Func<TEntity, object>> predicate)
        {
            OrderByQuery += string.Format(" ORDER BY {0} DESC", OrmHelper.getInstance.GetExpressionPropertyName<TEntity>(predicate));

            return this;
        }

        public override DBProviderBase<TEntity> OrderByAscending(Expression<Func<TEntity, object>> predicate)
        {
            OrderByQuery += string.Format(" ORDER BY {0} ASC", OrmHelper.getInstance.GetExpressionPropertyName<TEntity>(predicate));

            return this;
        }

        public override DBProviderBase<TEntity> Take(int count)
        {
            TakeQuery = string.Format(" TOP {0} ", count);

            return this;
        }

        public override DBProviderBase<TEntity> AddToCache(object cacheKey)
        {
            IsCacheActive = true;
            CacheKey = cacheKey;
            EExpirationType = CacheManager.EExpirationType.Untimed;

            return this;
        }

        public override DBProviderBase<TEntity> AddToCache(object cacheKey, CacheManager.EExpirationType expirationType, DateTime expirationDate)
        {
            IsCacheActive = true;
            CacheKey = cacheKey;
            EExpirationType = CacheManager.EExpirationType.Expiration;
            ExpirationDate = expirationDate;

            return this;
        }

        public override DBProviderBase<TEntity> AddToCache(object cacheKey, CacheManager.EExpirationType expirationType, TimeSpan slidingExpirationTime)
        {
            IsCacheActive = true;
            CacheKey = cacheKey;
            EExpirationType = CacheManager.EExpirationType.SlidingExpiration;
            SlidingExpirationTime = slidingExpirationTime;

            return this;
        }

        public override TEntity FirstOrDefault()
        {
            SelectStatement = SelectStatement.Insert(7, " TOP 1 ");
            WhereClause += OrderByQuery;

            var result = ExecuteSelectQuery();

            return result.FirstOrDefault();
        }

        public override List<TEntity> ToList()
        {
            if (!string.IsNullOrEmpty(TakeQuery))
                SelectStatement = SelectStatement.Insert(7, TakeQuery);

            WhereClause += OrderByQuery;

            var result = ExecuteSelectQuery();

            return result;
        }
        #endregion
        #endregion

        #region ITableCreation Members
        public void CreateTables()
        {
            #region Variables
            StringBuilder tableSql = new StringBuilder();
            StringBuilder addForeignKeyConstraintsSql = new StringBuilder();
            string row = string.Empty;
            bool IsModifiedColumn = false;
            object valueType = new object();
            Type entityType;
            List<dynamic> implementedEntities = (from property in EntityType.GetProperties()
                                                 from genericArguments in property.PropertyType.GetGenericArguments()
                                                 where genericArguments.BaseType.Equals(typeof(ModelBase))
                                                 select Activator.CreateInstance(genericArguments)).ToList();
            #endregion

            #region Drop Foreign Key Constraints
            DropForeignKeyConstraints(tableSql, addForeignKeyConstraintsSql);
            #endregion

            #region Drop Tables
            foreach (var entity in implementedEntities)
            {
                entityType = entity.GetType();

                tableSql.Append(String.Format("IF OBJECT_ID(N'[dbo].[{0}]', 'U') IS NOT NULL DROP TABLE [dbo].[{1}]; ", entityType.Name, entityType.Name));
            }
            #endregion

            #region Create Tables
            foreach (var entity in implementedEntities)
            {
                entityType = entity.GetType();

                #region Add Columns
                tableSql.Append(string.Format("CREATE TABLE [dbo].[{0}] (", entityType.Name));

                foreach (var loopProperty in entityType.GetProperties())
                {
                    valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        continue;
                    }

                    IsModifiedColumn = false;
                    row = loopProperty.Name;

                    #region Find Property Value Type and Add to Row
                    valueType = loopProperty.GetCustomAttributes(typeof(IsAutoIncrement), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        tableSql.Append(row + " int IDENTITY(" + ((IsAutoIncrement)(valueType)).StartPoint + "," + ((IsAutoIncrement)(valueType)).Length + ") ,");
                        continue;
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(INT), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " int";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(TINYINT), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " tinyint";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(SMALLINT), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " smallint";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(BOOLEAN), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " bit";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(DATETIME), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " DATETIME";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(IMAGE), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " image";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(MONEY), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " money";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(DECIMAL), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " DECIMAL(" + ((DECIMAL)(valueType)).Stair + "," + ((DECIMAL)(valueType)).Comma + ")";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NVARCHAR), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += ((NVARCHAR)valueType).Length > 0 ? String.Concat(" nvarchar(", ((NVARCHAR)valueType).Length, ")") : String.Concat(" nvarchar(", ((NVARCHAR)valueType).Max, ")");
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(VARCHAR), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += ((VARCHAR)valueType).Length > 0 ? String.Concat(" varchar(", ((VARCHAR)valueType).Length, ")") : String.Concat(" varchar(", ((VARCHAR)valueType).Max, ")");
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NCHAR), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += String.Concat(" nchar(", ((NCHAR)valueType).Length, ")");
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NTEXT), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " ntext";
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NULL), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        row += " NULL,";
                        IsModifiedColumn = true;
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NOTNULL), false).FirstOrDefault();
                    if (valueType != null && !IsModifiedColumn)
                    {
                        row += " NOT NULL,";
                        IsModifiedColumn = true;
                    }

                    if (!IsModifiedColumn)
                        row += ",";
                    #endregion

                    if (row != loopProperty.Name)
                        tableSql.Append(row);
                }

                tableSql.Append("); ");
                #endregion

                #region Add Indexes
                foreach (var loopProperty in entityType.GetProperties())
                {
                    valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        continue;
                    }

                    valueType = loopProperty.GetCustomAttributes(typeof(NONCLUSTEREDINDEX), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        tableSql.Append("CREATE NONCLUSTERED INDEX NCI_" + entityType.Name + "_" + loopProperty.Name + " ON [dbo].[" + entityType.Name + "] ([" + loopProperty.Name + "] ASC );");
                    }
                }
                #endregion
            }

            tableSql = tableSql.Replace(",);", ");");
            #endregion

            #region Add Primary Key Constraints
            foreach (var entity in implementedEntities)
            {
                entityType = entity.GetType();

                foreach (var loopProperty in entityType.GetProperties())
                {
                    valueType = loopProperty.GetCustomAttributes(typeof(IsPrimaryKey), false).FirstOrDefault();
                    if (valueType != null)
                    {
                        tableSql.Append("ALTER TABLE [dbo].[" + entityType.Name + "] ADD CONSTRAINT PK_" + entityType.Name + loopProperty.Name + " PRIMARY KEY (" + loopProperty.Name + "); ");
                        break;
                    }
                }
            }
            #endregion

            #region Add Foreign Key Constraints
            tableSql.Append(addForeignKeyConstraintsSql.ToString());
            #endregion

            DBManager.CommandText = tableSql.ToString();

            SubmitChanges();
        }

        public void CreateOrAlterTable()
        {
            #region Variables
            StringBuilder tableSql = new StringBuilder();
            StringBuilder addForeignKeyConstraintsSql = new StringBuilder();
            string row = string.Empty;
            bool IsModifiedColumn = false;
            object valueType = new object();
            #endregion

            #region Drop Foreign Key Constraints
            DropForeignKeyConstraints(tableSql, addForeignKeyConstraintsSql);
            #endregion

            #region Drop Tables
            tableSql.Append(String.Format("IF OBJECT_ID(N'[dbo].[{0}]', 'U') IS NOT NULL DROP TABLE [dbo].[{1}]; ", EntityType.Name, EntityType.Name));
            #endregion

            #region Create Tables
            #region Add Columns
            tableSql.Append(string.Format("CREATE TABLE [dbo].[{0}] (", EntityType.Name));

            foreach (var loopProperty in EntityType.GetProperties())
            {
                valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                IsModifiedColumn = false;
                row = loopProperty.Name;

                #region Find Property Value Type and Add to Row
                valueType = loopProperty.GetCustomAttributes(typeof(IsAutoIncrement), false).FirstOrDefault();
                if (valueType != null)
                {
                    tableSql.Append(row + " int IDENTITY(" + ((IsAutoIncrement)(valueType)).StartPoint + "," + ((IsAutoIncrement)(valueType)).Length + ") ,");
                    continue;
                }

                valueType = loopProperty.GetCustomAttributes(typeof(INT), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " int";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(TINYINT), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " tinyint";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(SMALLINT), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " smallint";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(BOOLEAN), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " bit";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(DATETIME), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " DATETIME";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(IMAGE), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " image";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(MONEY), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " money";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(DECIMAL), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " DECIMAL(" + ((DECIMAL)(valueType)).Stair + "," + ((DECIMAL)(valueType)).Comma + ")";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NVARCHAR), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += ((NVARCHAR)valueType).Length > 0 ? String.Concat(" nvarchar(", ((NVARCHAR)valueType).Length, ")") : String.Concat(" nvarchar(", ((NVARCHAR)valueType).Max, ")");
                }

                valueType = loopProperty.GetCustomAttributes(typeof(VARCHAR), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += ((VARCHAR)valueType).Length > 0 ? String.Concat(" varchar(", ((VARCHAR)valueType).Length, ")") : String.Concat(" varchar(", ((VARCHAR)valueType).Max, ")");
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NCHAR), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += String.Concat(" nchar(", ((NCHAR)valueType).Length, ")");
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NTEXT), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " ntext";
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NULL), false).FirstOrDefault();
                if (valueType != null)
                {
                    row += " NULL,";
                    IsModifiedColumn = true;
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NOTNULL), false).FirstOrDefault();
                if (valueType != null && !IsModifiedColumn)
                {
                    row += " NOT NULL,";
                    IsModifiedColumn = true;
                }

                if (!IsModifiedColumn)
                    row += ",";
                #endregion

                if (row != loopProperty.Name)
                    tableSql.Append(row);
            }

            tableSql.Append("); ");
            #endregion

            #region Add Indexes
            foreach (var loopProperty in EntityType.GetProperties())
            {
                valueType = loopProperty.GetCustomAttributes(typeof(IsRelationEntity), false).FirstOrDefault();
                if (valueType != null)
                {
                    continue;
                }

                valueType = loopProperty.GetCustomAttributes(typeof(NONCLUSTEREDINDEX), false).FirstOrDefault();
                if (valueType != null)
                {
                    tableSql.Append("CREATE NONCLUSTERED INDEX NCI_" + EntityType.Name + "_" + loopProperty.Name + " ON [dbo].[" + EntityType.Name + "] ([" + loopProperty.Name + "] ASC );");
                }
            }
            #endregion

            tableSql = tableSql.Replace(",);", ");");
            #endregion

            #region Add Primary Key Constraints
            foreach (var loopProperty in EntityType.GetProperties())
            {
                valueType = loopProperty.GetCustomAttributes(typeof(IsPrimaryKey), false).FirstOrDefault();
                if (valueType != null)
                {
                    tableSql.Append("ALTER TABLE [dbo].[" + EntityType.Name + "] ADD CONSTRAINT PK_" + EntityType.Name + loopProperty.Name + " PRIMARY KEY (" + loopProperty.Name + "); ");
                    break;
                }
            }
            #endregion

            #region Add Foreign Key Constraints
            tableSql.Append(addForeignKeyConstraintsSql);
            #endregion

            DBManager.CommandText = tableSql.ToString();

            SubmitChanges();
        }

        public void DropTable(bool forceDrop = false)
        {
            #region Variables
            StringBuilder tableSql = new StringBuilder();
            #endregion

            #region Drop Foreign Key Constraints
            if (forceDrop)
            {
                DropForeignKeyConstraints(tableSql);
            }
            #endregion

            #region Drop Table
            tableSql.Append(String.Format("IF OBJECT_ID(N'[dbo].[{0}]', 'U') IS NOT NULL DROP TABLE [dbo].[{1}]; ", EntityType.Name, EntityType.Name));
            #endregion

            DBManager.CommandText = tableSql.ToString();

            SubmitChanges();
        }

        public void TruncateTable(bool forceTruncate = false)
        {
            #region Variables
            StringBuilder tableSql = new StringBuilder();
            #endregion

            #region Drop Foreign Key Constraints
            if (forceTruncate)
            {
                DropForeignKeyConstraints(tableSql);
            }
            #endregion

            #region Truncate Table
            tableSql.Append(String.Format("TRUNCATE TABLE [dbo].[{0}]; ", EntityType.Name));
            #endregion

            DBManager.CommandText = tableSql.ToString();

            SubmitChanges();
        }
        #endregion

        #region Public Methods
        public void SetOrmConfiguration(OrmConfiguration ormConfiguration)
        {
            this._ormConfiguration = ormConfiguration;
        }
        #endregion

        #region Private Methods
        private List<TEntity> ExecuteSelectQuery()
        {
            List<TEntity> entities = new List<TEntity>();
            TEntity entity = default(TEntity);
            Type entityType;

            var dbManager = new DBManager(_ormConfiguration);

            if (!string.IsNullOrEmpty(WhereClause))
                SelectStatement = string.Format("{0} WHERE {1}", SelectStatement, WhereClause);

            string commandText = string.Format(SelectStatement, typeof(TEntity).Name);
            dbManager.CreateCommand(commandText);

            using (var reader = dbManager.ExecuteReader())
            {
                while (reader.Read())
                {
                    entity = new TEntity();
                    entityType = entity.GetType();

                    foreach (var loopProp in entityType.GetProperties().Where(p => p.GetCustomAttributes(typeof(IsRelationEntity), false).Count() == 0))
                    {
                        var propName = loopProp.Name;
                        var propValue = reader[propName];

                        if (propValue == DBNull.Value)
                            propValue = null;

                        loopProp.SetValue(entity, propValue);
                    }

                    entityType.GetMethod("SetOrmConfiguration").Invoke(entity, new object[] { _ormConfiguration });

                    entities.Add(entity);
                }
            }

            #region Add Cache
            if (IsCacheActive)
            {
                CacheManager.getInstance.AddOrUpdate(CacheKey, entities);
            }
            #endregion

            CleanFluentProperties();

            return entities;
        }

        private void DropForeignKeyConstraints(StringBuilder tableSql, StringBuilder addForeignKeyConstraintsSql = null)
        {
            IsForeignKey IsForeignKeyType;
            Type entityType;
            List<dynamic> haveRelatedForeignKeyEntities = (from property in AppDomain.CurrentDomain.GetAssemblies()
                                                           from type in property.GetTypes()
                                                           where type.GetInterfaces().Contains(typeof(ModelBase)) && type.GetConstructor(Type.EmptyTypes) != null
                                                           select Activator.CreateInstance(type)).ToList();

            foreach (var entity in haveRelatedForeignKeyEntities)
            {
                entityType = entity.GetType();

                foreach (var loopProperty in entityType.GetProperties())
                {
                    IsForeignKeyType = loopProperty.GetCustomAttributes(typeof(IsForeignKey), false).FirstOrDefault() as IsForeignKey;

                    if (IsForeignKeyType != null)
                    {
                        tableSql.Append(String.Format("IF OBJECT_ID(N'[dbo].[FK_{0}_{1}]', 'F') IS NOT NULL ALTER TABLE [dbo].[{2}] DROP CONSTRAINT [FK_{3}_{4}]; ", entityType.Name, loopProperty.Name, entityType.Name, entityType.Name, loopProperty.Name));

                        if (addForeignKeyConstraintsSql != null)
                            addForeignKeyConstraintsSql.Append("ALTER TABLE [dbo].[" + entityType.Name + "] ADD CONSTRAINT FK_" + entityType.Name + "_" + loopProperty.Name + " FOREIGN KEY (" + loopProperty.Name + ") REFERENCES " + ((IsForeignKey)IsForeignKeyType).ForeignTableName + "(" + ((IsForeignKey)IsForeignKeyType).ForeignColumnName + "); ");
                    }
                }
            }
        }

        protected override int SubmitChanges()
        {
            var dbManager = new DBManager(_ormConfiguration);

            dbManager.CreateCommand(DBManager.CommandText);

            if (DBManager.CommandParameters != null && DBManager.CommandParameters.Count > 0)
                foreach (var loopParameter in DBManager.CommandParameters)
                {
                    dbManager.AddParameter(loopParameter);
                }

            int status = dbManager.ExecuteNonQuery();

            return status;
        }
        #endregion
    }
}