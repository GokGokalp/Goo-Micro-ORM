using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Shippers : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int ShipperID { get; set; }

        [NVARCHAR(40)]
        [NOTNULL]
        public string CompanyName { get; set; }

        [NVARCHAR(24)]
        [NULL]
        public string Phone { get; set; }
    }
}