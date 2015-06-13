using Goo.Attributes;
using Goo.OrmCore;
using System;

namespace Goo.UnitTest.Entities
{
    public class Orders : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int OrderID { get; set; }

        [IsForeignKey("Customers", "CustomerID")]
        [NCHAR(5)]
        [NULL]
        public string CustomerID { get; set; }

        [IsForeignKey("Employees", "EmployeeID")]
        [INT]
        [NULL]
        public System.Nullable<int> EmployeeID { get; set; }

        [DATETIME]
        [NULL]
        public System.Nullable<DateTime> OrderDate { get; set; }

        [DATETIME]
        [NULL]
        public System.Nullable<DateTime> RequiredDate { get; set; }

        [DATETIME]
        [NULL]
        public System.Nullable<DateTime> ShippedDate { get; set; }

        [INT]
        [NULL]
        public System.Nullable<int> ShipVia { get; set; }

        [MONEY]
        [NULL]
        public System.Nullable<decimal> Freight { get; set; }

        [NVARCHAR(40)]
        [NULL]
        public string ShipName { get; set; }

        [NVARCHAR(60)]
        [NULL]
        public string ShipAddress { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string ShipCity { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string ShipRegion { get; set; }

        [NVARCHAR(10)]
        [NULL]
        public string ShipPostalCode { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string ShipCountry { get; set; }

        // Relations
        [IsRelationEntity]
        public Employees Employe { get { return base.GetRelation<Employees>("Employees", EmployeeID); } set { Employe = value; } }

        [IsRelationEntity]
        public Customers Customer { get { return base.GetRelation<Customers>("CustomerID", CustomerID); } set { Customer = value; } }
    }
}