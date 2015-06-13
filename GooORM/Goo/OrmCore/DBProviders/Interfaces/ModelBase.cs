using Goo.CacheManagement;
using Goo.Helpers;
using System.Collections.Generic;

namespace Goo.OrmCore
{
    public abstract class ModelBase
    {
        #region Private Members
        private OrmConfiguration _ormConfiguration;
        #endregion

        #region Public Methods
        /// <summary>
        /// Eğer contextin lazy loading özelliği etkinleştirilmiş ise ilişkili nesnelere erişilmeye çalışıldığında getirmeyi sağlar.
        /// This method provides to get relation of entity, if the related context's lazy loading property is enabled.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="foreignColumnName">Foreign column name.</param>
        /// <returns></returns>
        public TEntity GetRelation<TEntity>(string foreignColumnName, object value)
        {
            if (_ormConfiguration.LazyLoadingEnabled)
            {
                if (string.IsNullOrEmpty(foreignColumnName) || value == null)
                    return default(TEntity);

                string key = string.Format("{0}|{1}|{2}", typeof(TEntity).Name, foreignColumnName, value);

                if (CacheManager.getInstance.Contains(key))
                {
                    return OrmHelper.getInstance.ConvertTo<TEntity>(CacheManager.getInstance.Get(key));
                }
                else
                {
                    var provider = DBProviderFactory<TEntity>.GetDbProvider(_ormConfiguration);

                    var relationEntity = provider.GetRelation(foreignColumnName, value);

                    CacheManager.getInstance.AddOrUpdate(key, relationEntity);

                    return relationEntity;
                }
            }
            else
                return default(TEntity);
        }

        /// <summary>
        /// Eğer contextin lazy loading özelliği etkinleştirilmiş ise ilişkili nesnelere erişilmeye çalışıldığında getirmeyi sağlar.
        /// This method provides to get relation of entities, if the related context's lazy loading property is enabled.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="foreignColumnName">Foreign column name.</param>
        /// <returns></returns>
        public IList<TEntity> GetRelations<TEntity>(string foreignColumnName, object value)
        {
            if (_ormConfiguration.LazyLoadingEnabled)
            {
                if (string.IsNullOrEmpty(foreignColumnName) || value == null)
                    return new List<TEntity>();

                string key = string.Format("{0}|{1}|{2}", typeof(TEntity).Name, foreignColumnName, value);

                if (CacheManager.getInstance.Contains(key))
                {
                    return OrmHelper.getInstance.ConvertTo<List<TEntity>>(CacheManager.getInstance.Get(key));
                }
                else
                {
                    var provider = DBProviderFactory<TEntity>.GetDbProvider(_ormConfiguration);

                    var relationEntities = provider.GetRelations(foreignColumnName, value);

                    CacheManager.getInstance.AddOrUpdate(key, relationEntities);

                    return relationEntities;
                }
            }
            else
                return new List<TEntity>();
        }

        public void SetOrmConfiguration(OrmConfiguration ormConfiguration)
        {
            this._ormConfiguration = ormConfiguration;
        }
        #endregion
    }
}