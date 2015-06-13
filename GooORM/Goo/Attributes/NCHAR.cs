using System;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NCHAR : Attribute
    {
        private int _length;

        public NCHAR(int length = 0)
        {
            this._length = length;
        }

        public int Length
        {
            get { return _length; }
            set { this._length = value; }
        }
    }
}