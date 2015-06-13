using Goo.Attributes;
using Goo.OrmCore;
using System;

namespace Goo.UnitTest.Entities
{
    public class Employees : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int EmployeeID { get; set; }

        [NVARCHAR(20)]
        [NOTNULL]
        public string LastName { get; set; }

        [NVARCHAR(10)]
        [NOTNULL]
        public string FirstName { get; set; }

        [NVARCHAR(30)]
        [NULL]
        public string Title { get; set; }

        [NVARCHAR(25)]
        [NULL]
        public string TitleOfCourtesy { get; set; }

        [DATETIME]
        [NULL]
        public System.Nullable<DateTime> BirthDate { get; set; }

        [DATETIME]
        [NULL]
        public System.Nullable<DateTime> HireDate { get; set; }

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
        public string HomePhone { get; set; }

        [NVARCHAR(4)]
        [NULL]
        public string Extension { get; set; }

        [IMAGE]
        [NULL]
        public byte[] Photo { get; set; }

        [NTEXT]
        [NULL]
        public string Notes { get; set; }

        [INT]
        [NULL]
        public System.Nullable<int> ReportsTo { get; set; }

        [NVARCHAR(255)]
        [NULL]
        public string PhotoPath { get; set; }
    }
}