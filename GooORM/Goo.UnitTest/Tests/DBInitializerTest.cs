using Microsoft.VisualStudio.TestTools.UnitTesting;
using Goo.OrmCore;
using Goo.UnitTest.Entities;

namespace Goo.UnitTest.Tests
{
    /// <summary>
    /// DBInitializerManager sınıfı ile veritabanı üzerinde drop/alter/truncate gibi işlemleri yapabilir veya eklediğiniz nesneler üzerinden veritabanını oluşturabilirsiniz.
    /// Using the DBInitializerManager class you can create your database or perform basic operations like drop/alter/truncate.
    /// </summary>
    [TestClass]
    public class DBInitializerTest
    {
        private DBInitializerManager dbInitializerManager;

        [TestInitialize]
        public void TestInitialize()
        {
            dbInitializerManager = DBInitializerManager.getInstance;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            dbInitializerManager = null;
        }

        #region Table Test Methods
        /// <summary>
        /// Eğer nesneniz ModelBase soyut sınıfından kalıtım alıyor ise bu metot veritabanınızı oluşturmayı sağlar.
        /// This method initializes the database, only if the entities inherit the ModelBase abstract class.
        /// </summary>
        [TestMethod]
        public void InitializeDatabase()
        {
            dbInitializerManager.InitializeDatabase<GooContext>();
        }

        /// <summary>
        /// Bu metot tabloyu drop edebilmenizi sağlar.
        /// This method drops the table.
        /// </summary>
        [TestMethod]
        public void DropTable()
        {
            dbInitializerManager.DropTable<Categories>();
        }

        /// <summary>
        /// Eğer nesneniz ModelBase soyut sınıfından kalıtım alıyor ise bu metot tabloyu create veya alter edebilmenizi sağlar.
        /// This method runs the create/alter table, only if the entity inherit the ModelBase abstract class.
        /// </summary>
        [TestMethod]
        public void CreateOrAlterTable()
        {
            dbInitializerManager.CreateOrAlterTable<Categories>();
        }

        /// <summary>
        /// Bu metot tabloyu truncate edebilmenizi sağlar.
        /// This method truncates table.
        /// </summary>
        [TestMethod]
        public void TruncateTable()
        {
            dbInitializerManager.TruncateTable<Categories>();
        }
        #endregion
    }
}