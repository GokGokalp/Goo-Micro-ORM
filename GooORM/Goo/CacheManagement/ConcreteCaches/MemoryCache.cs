using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Goo.CacheManagement
{
    internal class MemoryCache : CacheBase
    {
        #region Private Members
        private System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromSeconds(1.0).TotalMilliseconds);
        private SortedDictionary<TimedCacheKey, object> timedStorage = new SortedDictionary<TimedCacheKey, object>();
        private Dictionary<object, TimedCacheKey> timedStorageIndex = new Dictionary<object, TimedCacheKey>();
        private Hashtable untimedStorage = new Hashtable();
        object isPurging = new object();
        #endregion

        #region Constructor
        public MemoryCache()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(PurgeCache);
            timer.Start();
        }
        #endregion

        #region CacheBase Members
        protected override bool Add(object key, object value)
        {
            if (untimedStorage.ContainsKey(key))
                return false;
            else
            {
                untimedStorage.Add(key, value);
                return true;
            }
        }

        protected override bool Add(object key, object value, DateTime expirationDate)
        {
            if (timedStorageIndex.ContainsKey(key))
                return false;
            else
            {
                TimedCacheKey cacheKey = new TimedCacheKey(key, expirationDate);

                timedStorage.Add(cacheKey, value);
                timedStorageIndex.Add(key, cacheKey);

                return true;
            }
        }

        protected override bool Add(object key, object value, TimeSpan slidingExpirationTime)
        {
            if (timedStorageIndex.ContainsKey(key))
                return false;
            else
            {
                TimedCacheKey cacheKey = new TimedCacheKey(key, slidingExpirationTime);

                timedStorage.Add(cacheKey, value);
                timedStorageIndex.Add(key, cacheKey);

                return true;
            }
        }

        protected override bool Update(object key, object value)
        {
            if (untimedStorage.ContainsKey(key))
            {
                untimedStorage.Remove(key);
                untimedStorage.Add(key, value);

                return true;
            }
            else if (timedStorageIndex.ContainsKey(key))
            {
                timedStorage.Remove(timedStorageIndex[key]);
                timedStorageIndex[key].Accessed();
                timedStorage.Add(timedStorageIndex[key], value);

                return true;
            }

            return false;
        }

        protected override bool Update(object key, object value, DateTime expirationDate)
        {
            if (untimedStorage.ContainsKey(key))
            {
                untimedStorage.Remove(key);
            }
            else if (timedStorageIndex.ContainsKey(key))
            {
                timedStorage.Remove(timedStorageIndex[key]);
                timedStorageIndex.Remove(key);
            }
            else
                return false;

            TimedCacheKey cacheKey = new TimedCacheKey(key, expirationDate);
            timedStorage.Add(cacheKey, value);
            timedStorageIndex.Add(key, cacheKey);

            return true;
        }

        protected override bool Update(object key, object value, TimeSpan slidingExpirationTime)
        {
            if (untimedStorage.ContainsKey(key))
            {
                untimedStorage.Remove(key);
            }
            else if (timedStorageIndex.ContainsKey(key))
            {
                timedStorage.Remove(timedStorageIndex[key]);
                timedStorageIndex.Remove(key);
            }
            else
            {
                return false;
            }

            TimedCacheKey cacheKey = new TimedCacheKey(key, slidingExpirationTime);
            timedStorage.Add(cacheKey, value);
            timedStorageIndex.Add(key, cacheKey);

            return true;
        }

        protected override object GetTimed(object key)
        {
            if (timedStorageIndex.ContainsKey(key))
            {
                Object cacheVal;
                TimedCacheKey cacheKey = timedStorageIndex[key];

                cacheVal = timedStorage[cacheKey];

                timedStorage.Remove(cacheKey);
                cacheKey.Accessed();

                timedStorage.Add(cacheKey, cacheVal);

                return cacheVal;
            }
            else
                return null;
        }

        protected override object GetUntimed(object key)
        {
            if (untimedStorage.ContainsKey(key))
                return untimedStorage[key];
            else
                return null;
        }

        public override bool ContainsCache(object key)
        {
            return untimedStorage.ContainsKey(key) || timedStorageIndex.ContainsKey(key);
        }

        protected override bool Remove(object key)
        {
            if (untimedStorage.ContainsKey(key))
            {
                untimedStorage.Remove(key);
                return true;
            }
            else if (timedStorageIndex.ContainsKey(key))
            {
                timedStorage.Remove(timedStorageIndex[key]);
                timedStorageIndex.Remove(key);
                return true;
            }
            else
                return false;
        }

        protected override void PurgeCache(object sender, System.Timers.ElapsedEventArgs e)
        {
            ReaderWriterLock readWriteLock = new ReaderWriterLock();
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; // This process is not critical as according to add or update.

            if (!Monitor.TryEnter(isPurging))
                return;

            try
            {
                readWriteLock.AcquireWriterLock(MAX_LOCK_WAIT);

                try
                {
                    List<object> expiredCacheObjects = new List<object>();

                    foreach (TimedCacheKey loopExpiredCacheObject in timedStorage.Keys)
                    {
                        if (loopExpiredCacheObject.ExpirationDate < e.SignalTime)
                        {
                            expiredCacheObjects.Add(loopExpiredCacheObject);
                        }
                        else
                            break;
                    }

                    foreach (object key in expiredCacheObjects)
                    {
                        TimedCacheKey timedCacheKey = timedStorageIndex[key];
                        timedStorageIndex.Remove(timedCacheKey.Key);

                        timedStorage.Remove(timedCacheKey);
                    }
                }
                finally
                {
                    readWriteLock.ReleaseWriterLock();
                }
            }
            finally
            {
                Monitor.Exit(isPurging);
            }
        }
        #endregion
    }
}