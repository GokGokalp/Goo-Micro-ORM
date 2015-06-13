using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Products : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public string ProductID { get; set; }

        [IsForeignKey("Suppliers", "SupplierID")]
        [INT]
        [NULL]
        public System.Nullable<int> SupplierID { get; set; }

        [IsForeignKey("Categories", "CategoryID")]
        [INT]
        [NULL]
        public System.Nullable<int> CategoryID { get; set; }

        [NVARCHAR(40)]
        [NOTNULL]
        public string ProductName { get; set; }

        [NVARCHAR(20)]
        [NULL]
        public string QuantityPerUnit { get; set; }

        [MONEY]
        [NULL]
        public System.Nullable<decimal> UnitPrice { get; set; }

        [SMALLINT]
        [NULL]
        public System.Nullable<short> UnitsInStock { get; set; }

        [SMALLINT]
        [NULL]
        public System.Nullable<short> UnitsOnOrder { get; set; }

        [SMALLINT]
        [NULL]
        public System.Nullable<short> ReorderLevel { get; set; }

        [BOOLEAN]
        [NULL]
        public System.Nullable<bool> Discontinued { get; set; }

        // Relations
        [IsRelationEntity]
        public Suppliers Supplier { get { return base.GetRelation<Suppliers>("SupplierID", SupplierID); } set { Supplier = value; } }

        [IsRelationEntity]
        public Categories Category { get { return base.GetRelation<Categories>("CategoryID", CategoryID); } set { Category = value; } }
    }
}