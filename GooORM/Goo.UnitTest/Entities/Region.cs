using Goo.Attributes;
using Goo.OrmCore;
using System.Collections.Generic;

namespace Goo.UnitTest.Entities
{
    public class Region : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int RegionID { get; set; }

        [NCHAR(50)]
        [NOTNULL]
        public string RegionDescription { get; set; }

        // Relations
        [IsRelationEntity]
        public IList<Territories> Territories { get { return base.GetRelations<Territories>("RegionID", RegionID); } set { Territories = value; } }
    }
}