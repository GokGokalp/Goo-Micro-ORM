using System.Collections.Generic;

namespace Goo.OrmCore
{
    public interface IDBTableCreationable
    {
        void CreateTables();
        void CreateOrAlterTable();
        void DropTable(bool forceDrop = false);
        void TruncateTable(bool forceTruncate = false);
    }
}