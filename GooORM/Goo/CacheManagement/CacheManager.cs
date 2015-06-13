using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goo.CacheManagement
{
    public class CacheManager
    {
        #region Private Members
        private CacheBase memoryCache;
        #endregion

        #region Constructor
        private static Lazy<CacheManager> _instance = new Lazy<CacheManager>(() => new CacheManager());
        public static CacheManager getInstance
        {
            get
            {
                return _instance.Value;
            }
        }

        private CacheManager()
        {
            memoryCache = new MemoryCache();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method provides to get an object from the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>The cached object.</returns>
        public object Get(object key)
        {
            return memoryCache.GetCache(key);
        }

        /// <summary>
        /// This method provides to add or update object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdate(object key, object value)
        {
            return memoryCache.AddOrUpdateCache(key, value);
        }

        /// <summary>
        /// This method provides to add or update object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <param name="expiration">The expiration time is determines when the object will remove from the cache.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdate(object key, object value, DateTime expiration)
        {
            return memoryCache.AddOrUpdateCache(key, value, expiration);
        }

        /// <summary>
        /// This method provides to add or update object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <param name="slidingExpiration">The sliding expiration time determines the remove time of object from cache before access.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdate(object key, object value, TimeSpan slidingExpiration)
        {
            return memoryCache.AddOrUpdateCache(key, value, slidingExpiration);
        }

        /// <summary>
        /// This method provides to check the object is in cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>true if the object removed successfully, otherwise false.</returns>
        public bool Contains(object key)
        {
            return memoryCache.ContainsCache(key);
        }

        /// <summary>
        /// This method provides to remove an object from the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>true if the object removed successfully, otherwise false.</returns>
        public bool Remove(object key)
        {
            return memoryCache.RemoveCache(key);
        }
        #endregion

        #region Enums
        public enum EExpirationType
        {
            Untimed,
            Expiration,
            SlidingExpiration
        }
        #endregion
    }
}