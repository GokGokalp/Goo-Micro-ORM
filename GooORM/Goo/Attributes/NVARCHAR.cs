using System;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NVARCHAR : Attribute
    {
        private const string maxLength = "MAX";
        private int _length;

        public NVARCHAR(int length = 0)
        {
            this._length = length;
        }

        public string Max
        {
            get { return maxLength; }
        }

        public int Length
        {
            get { return _length; }
            set { this._length = value; }
        }
    }
}
