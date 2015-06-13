using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Customers : ModelBase
    {
        [IsPrimaryKey]
        [NCHAR(5)]
        [NOTNULL]
        public string CustomerID { get; set; }

        [NVARCHAR(40)]
        [NOTNULL]
        public string CompanyName { get; set; }

        [NVARCHAR(30)]
        [NULL]
        public string ContactName { get; set; }

        [NVARCHAR(30)]
        [NULL]
        public string ContactTitle { get; set; }

        [NVARCHAR(60)]
        [NULL]
        public string Address { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string City { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string Region { get; set; }

        [NVARCHAR(10)]
        [NULL]
        public string PostalCode { get; set; }

        [NVARCHAR(15)]
        [NULL]
        public string Country { get; set; }

        [NVARCHAR(24)]
        [NULL]
        public string Phone { get; set; }

        [NVARCHAR(24)]
        [NULL]
        public string Fax { get; set; }
    }
}