using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Territories : ModelBase
    {
        [IsPrimaryKey]
        [NVARCHAR(20)]
        [NOTNULL]
        public string TerritoryID { get; set; }

        [IsForeignKey("Region", "RegionID")]
        [INT]
        [NOTNULL]
        public int RegionID { get; set; }

        [NCHAR(50)]
        [NOTNULL]
        public string TerritoryDescription { get; set; }

        // Relations
        [IsRelationEntity]
        public Region Region { get { return base.GetRelation<Region>("RegionID", RegionID); } set { Region = value; } }
    }
}