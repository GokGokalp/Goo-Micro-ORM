using System;

namespace Goo.CacheManagement
{
    internal class TimedCacheKey : IComparable
    {
        #region Private Members
        private DateTime _expirationDate;
        private bool _slidingExpiration;
        private TimeSpan _slidingExpirationWindowSize;
        private object _key;
        #endregion

        #region Public Methods
        public object Key
        {
            get { return _key; }
        }

        public DateTime ExpirationDate
        {
            get
            {
                return _expirationDate;
            }
        }

        public bool SlidingExpiration
        {
            get
            {
                return _slidingExpiration;
            }
        }

        public TimeSpan SlidingExpirationWindowSize
        {
            get
            {
                return _slidingExpirationWindowSize;
            }
        }

        public TimedCacheKey(object key, DateTime expirationDate)
        {
            _key = key;
            _slidingExpiration = false;
            _expirationDate = expirationDate;
        }

        public TimedCacheKey(object key, TimeSpan slidingExpirationWindowSize)
        {
            _key = key;
            _slidingExpiration = true;
            _slidingExpirationWindowSize = slidingExpirationWindowSize;

            Accessed();
        }

        public void Accessed()
        {
            if (_slidingExpiration)
            {
                _expirationDate = DateTime.Now.Add(_slidingExpirationWindowSize);
            }
        }
        #endregion

        #region IComparable Member
        public int CompareTo(object obj)
        {
            if (this == null && obj == null) { return 0; }
            if (this == null) { return -1; }
            if (obj == null) { return 1; }

            if (!(obj is TimedCacheKey))
            {
                throw new ArgumentException("The object is not a CacheItemKey");
            }

            int dateComparer = this._expirationDate.CompareTo((obj as TimedCacheKey)._expirationDate);
            if (dateComparer != 0)
            {
                return -dateComparer;
            }

            return this._key.GetHashCode().CompareTo((obj as TimedCacheKey)._key.GetHashCode());
        }
        #endregion
    }
}