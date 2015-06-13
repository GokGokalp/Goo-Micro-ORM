using System;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DECIMAL : Attribute
    {
        private int _stair;
        private int _comma;

        public DECIMAL(int stair, int comma)
        {
            this._stair = stair;
            this._comma = comma;
        }

        public int Stair
        {
            get { return _stair; }
            set { this._stair = value; }
        }

        public int Comma
        {
            get { return _comma; }
            set { this._comma = value; }
        }
    }
}