using System;
using System.Threading;
using System.Timers;

namespace Goo.CacheManagement
{
    internal abstract class CacheBase
    {
        #region Private Members
        private ReaderWriterLock readWriteLock = new ReaderWriterLock();
        internal const int MAX_LOCK_WAIT = 5000;
        #endregion

        #region Private Methods
        public T SurroundWithWriterLockOperation<T>(Func<T> func)
        {
            bool IsReaderLockHeld = readWriteLock.IsReaderLockHeld;
            LockCookie lCookie = new LockCookie();

            if (IsReaderLockHeld)
                lCookie = readWriteLock.UpgradeToWriterLock(MAX_LOCK_WAIT);
            else
                readWriteLock.AcquireWriterLock(MAX_LOCK_WAIT);

            try
            {
                return func();
            }
            finally
            {
                if (IsReaderLockHeld)
                    readWriteLock.DowngradeFromWriterLock(ref lCookie);
                else
                    readWriteLock.ReleaseWriterLock();
            }
        }

        public T SurroundWithReaderLockOperation<T>(Func<T> func)
        {
            readWriteLock.AcquireReaderLock(MAX_LOCK_WAIT);

            try
            {
                return func();
            }
            finally
            {
                readWriteLock.ReleaseReaderLock();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method provides to add object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdateCache(object key, object value)
        {
            return
            SurroundWithWriterLockOperation(() =>
            {
                if (ContainsCache(key))
                {
                    return Update(key, value);
                }
                else
                {
                    return Add(key, value);
                }
            });
        }

        /// <summary>
        /// This method provides to add object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <param name="expirationDate">The expiration time is determines when the object will remove from the cache.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdateCache(object key, object value, DateTime expirationDate)
        {
            return
            SurroundWithWriterLockOperation(() =>
            {
                if (ContainsCache(key))
                {
                   return Update(key, value, expirationDate);
                }
                else
                {
                   return Add(key, value, expirationDate);
                }
            });
        }

        /// <summary>
        /// This method provides to add object the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        /// <param name="slidingExpirationTime">The sliding expiration time determines the remove time of object from cache before access.</param>
        /// <returns>true if the object is added or updated to the cache be successfully, otherwise will return false.</returns>
        public bool AddOrUpdateCache(object key, object value, TimeSpan slidingExpirationTime)
        {
            return
            SurroundWithWriterLockOperation(() =>
            {
                if (ContainsCache(key))
                {
                    return Update(key, value, slidingExpirationTime);
                }
                else
                {
                    return Add(key, value, slidingExpirationTime);
                }
            });
        }

        /// <summary>
        /// This method provides to get an object from the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>The cached object.</returns>
        public object GetCache(object key)
        {
            object cacheObj = null;

            return
            SurroundWithReaderLockOperation(() =>
            {
                cacheObj = GetTimed(key);

                if (cacheObj != null) return cacheObj;
                else
                {
                    cacheObj = SurroundWithWriterLockOperation(() =>
                    {
                        return GetUntimed(key);
                    });

                    if (cacheObj != null) return cacheObj;
                    else return null;
                }
            });
        }

        /// <summary>
        /// This method provides to remove an object from the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>true if the object removed successfully, otherwise false.</returns>
        public bool RemoveCache(object key)
        {
            return
            SurroundWithReaderLockOperation(() =>
            {
                return Remove(key);
            });
        }
        #endregion

        #region Abstract Methods
        protected abstract void PurgeCache(object sender, ElapsedEventArgs e);
        protected abstract bool Add(object key, object value);
        protected abstract bool Add(object key, object value, DateTime expiration);
        protected abstract bool Add(object key, object value, TimeSpan slidingExpiration);
        protected abstract bool Update(object key, object value);
        protected abstract bool Update(object key, object value, DateTime expiration);
        protected abstract bool Update(object key, object value, TimeSpan slidingExpiration);
        protected abstract object GetTimed(object key);
        protected abstract object GetUntimed(object key);
        protected abstract bool Remove(object key);
        public abstract bool ContainsCache(object key);
        #endregion
    }
}