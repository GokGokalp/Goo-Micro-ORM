using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsAutoIncrement : Attribute
    {
        #region Fields
        private int _length;
        private int _startPoint;
        #endregion

        #region Constructor
        public IsAutoIncrement(int length, int startPoint)
        {
            this._length = length;
            this._startPoint = startPoint;
        }
        #endregion

        #region Properties
        public int Length
        {
            get { return _length; }
            set { this._length = value; }
        }

        public int StartPoint
        {
            get { return _startPoint; }
            set { this._startPoint = value; }
        }
        #endregion
    }
}