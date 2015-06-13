using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Categories : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int CategoryID { get; set; }

        [NVARCHAR(15)]
        [NOTNULL]
        public string CategoryName { get; set; }

        [NTEXT]
        [NULL]
        public string Description { get; set; }

        [IMAGE]
        [NULL]
        public byte[] Picture { get; set; }
    }
}