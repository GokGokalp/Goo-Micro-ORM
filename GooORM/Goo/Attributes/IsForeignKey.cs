using System;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsForeignKey : Attribute
    {
        #region Fields
        private string _foreignTableName;
        private string _foreignColumnName;
        #endregion

        #region Constructor
        public IsForeignKey(string foreignTableName, string foreignColumnName)
        {
            this._foreignTableName = foreignTableName;
            this._foreignColumnName = foreignColumnName;
        }
        #endregion

        #region Properties
        public string ForeignTableName
        {
            get { return _foreignTableName; }
            set { this._foreignTableName = value; }
        }

        public string ForeignColumnName
        {
            get { return _foreignColumnName; }
            set { this._foreignColumnName = value; }
        }
        #endregion
    }
}
