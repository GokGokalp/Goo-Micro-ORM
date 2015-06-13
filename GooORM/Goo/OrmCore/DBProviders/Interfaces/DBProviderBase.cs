using Goo.CacheManagement;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace Goo.OrmCore
{
    public abstract class DBProviderBase<TEntity>
    {
        #region Properties
        protected Type EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        protected DbConnection Connection;
        #endregion

        #region Fluent Properties
        private string _selectStatement = "SELECT * FROM {0}";
        protected string SelectStatement
        {
            get { return _selectStatement; }
            set { _selectStatement = value; }
        }
        protected string WhereClause { get; set; }
        protected string OrderByQuery { get; set; }
        protected string TakeQuery { get; set; }
        protected bool IsCacheActive { get; set; }
        protected object CacheKey { get; set; }
        protected CacheManager.EExpirationType EExpirationType { get; set; }
        protected DateTime ExpirationDate { get; set; }
        protected TimeSpan SlidingExpirationTime { get; set; }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Bu metot lambda ifadeleri aracılığı ile istediğiniz koşuldaki entityleri getirmenizi sağlar.
        /// This method uses lambda expressions to let you write your own queries.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> Where(Expression<Func<TEntity, object>> predicate);

        /// <summary>
        /// GetFromCache kullanarak daha önceden önbelleğe eklemiş olduğunuz nesneye erişmenizi sağlar. Nesne bulunamaması durumunda geriye null dönmektedir.
        /// Allows you to get the objects from the cache. If no objects are found, returns null.
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        /// <returns></returns>
        public abstract List<TEntity> GetFromCache(object cacheKey);

        internal abstract TEntity GetRelation(string foreignColumnName, object value);

        internal abstract List<TEntity> GetRelations(string foreignColumnName, object value);

        /// <summary>
        /// Bu metot nesne eklemenizi sağlar.
        /// This method inserts the entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public abstract void Insert(TEntity entity);

        /// <summary>
        /// Bu metot bir nesneyi güncellemenizi sağlar.
        /// This method updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public abstract void Update(TEntity entity);

        /// <summary>
        /// Bu metot bir nesne silmenizi sağlar.
        /// This method deletes an entity.
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Delete(TEntity entity);

        /// <summary>
        /// Ekleme, silme veya güncelleme gibi yapılmış olan işlemleri, veritabanında uygulamayı sağlar.
        /// Allows you to perform insert, delete or update operations on the database.
        /// </summary>
        /// <returns></returns>
        protected abstract int SubmitChanges();

        #region Fluent Methods
        /// <summary>
        /// Bu metot nesneler içindeki ilk nesneyi getirmeye yarar. Herhangi bir durumda geriye null değer döner.
        /// This method fetches the first row of the entity. Otherwise returns null.
        /// </summary>
        /// <returns></returns>
        public abstract TEntity FirstOrDefault();

        /// <summary>
        /// Bu metot tüm nesneleri listelemeye yarar. Herhangi bir durumda count 0 döner.
        /// This method returns the entity as a list. Otherwise returns 0.
        /// </summary>
        /// <returns></returns>
        public abstract List<TEntity> ToList();

        /// <summary>
        /// Azalarak sıralamayı sağlar.
        /// Order by descending.
        /// </summary>
        /// <param name="predicate">Property name.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> OrderByDescending(Expression<Func<TEntity, object>> predicate);

        /// <summary>
        /// Artarak sıralamayı sağlar.
        /// Order by ascending.
        /// </summary>
        /// <param name="predicate">Property name.</param>
        public abstract DBProviderBase<TEntity> OrderByAscending(Expression<Func<TEntity, object>> predicate);

        /// <summary>
        /// Nesneyi belirlenen bir miktarda getirmeyi sağlar.
        /// Returns specified amount of rows.
        /// </summary>
        /// <param name="count">Entity count.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> Take(int count);

        /// <summary>
        /// Nesneyi önbelleğe varsayılan olarak limitsiz eklemeyi sağlar.
        /// Adds the entity to the cache (non-expiry as default)
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> AddToCache(object cacheKey);

        /// <summary>
        /// Nesneyi önbelleğe belirlenen bir tarih boyunca eklemeyi sağlar.
        /// Adds the entity to the cache (Timed)
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="expirationType">Expiration type.</param>
        /// <param name="expirationDate">Expiration date.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> AddToCache(object cacheKey, CacheManager.EExpirationType expirationType, DateTime expirationDate);

        /// <summary>
        /// Nesneyi önbelleğe belirlenen bir süre boyunca eklemeyi sağlar.
        /// Adds the entity to the cache (Sliding timed)
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="expirationType">Expiration type.</param>
        /// <param name="slidingExpirationTime">Expiration time.</param>
        /// <returns></returns>
        public abstract DBProviderBase<TEntity> AddToCache(object cacheKey, CacheManager.EExpirationType expirationType, TimeSpan slidingExpirationTime);
        #endregion
        #endregion

        protected void CleanFluentProperties()
        {
            this.SelectStatement = "SELECT * FROM {0}";
            this.WhereClause = string.Empty;
            this.OrderByQuery = string.Empty;
            this.TakeQuery = string.Empty;
            this.IsCacheActive = false;
            this.CacheKey = null;
        }
    }
}