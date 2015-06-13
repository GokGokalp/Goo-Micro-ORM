using Microsoft.VisualStudio.TestTools.UnitTesting;
using Goo.UnitTest.Entities;

namespace Goo.UnitTest.Tests
{
    /// <summary>
    /// GooContext sınıfı üzerinden insert/update ve delete işlemlerinizi yapabilirsiniz.
    /// Using the GooContext class you can perform basic operations like insert/update or delete an entity.
    /// </summary>
    [TestClass]
    public class CrudTest
    {
        private GooContext gooContext;

        [TestInitialize]
        public void TestInitialize()
        {
            gooContext = new GooContext();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            gooContext = null;
        }

        #region Create/Update/Delete Test Methods
        /// <summary>
        /// Bu metot nesne eklemenizi sağlar.
        /// This method inserts the entity.
        /// </summary>
        [TestMethod]
        public void InsertAnEntity()
        {
            Categories category = new Categories()
            {
                CategoryName = "Computer",
                Description = "Insert test"
            };

            gooContext.Categories.Insert(category);

            int result = gooContext.SubmitChanges();

            Assert.AreNotEqual(result, -1);
        }

        /// <summary>
        /// Bu metot ile bir nesneyi transaction işlemi ile eklemenizi sağlar.
        /// This method inserts the entity with transaction.
        /// </summary>
        [TestMethod]
        public void InsertAnEntityWithTransaction()
        {
            // Connection'a erişmeniz gereken durumlarda bağlantıyı sizin açmanız gerekmektedir.
            // In case you need to reference the connection object, you need to open and manage the connection.
            gooContext.OrmConfiguration.Connection.Open();

            using (var transaction = gooContext.OrmConfiguration.Connection.BeginTransaction())
            {
                // Transaction nesnesini orm'e kullanacağımızı belirtmeliyiz.
                // You must pass in the transaction object's reference.
                gooContext.OrmConfiguration.UseTransaction(transaction);

                Categories category = new Categories()
                {
                    CategoryName = "Computer",
                    Description = "Insert test with transaction"
                };

                gooContext.Categories.Insert(category);

                int result = gooContext.SubmitChanges();

                if (result > -1)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        /// <summary>
        /// Bu metot bir nesneyi güncellemenizi sağlar.
        /// This method updates an entity.
        /// </summary>
        [TestMethod]
        public void UpdateAnEntity()
        {
            Categories category = gooContext.Categories.FirstOrDefault();

            category.CategoryName = "Computer Update";
            category.Description = "Update test";

            gooContext.Categories.Update(category);

            int result = gooContext.SubmitChanges();

            Assert.AreNotEqual(result, -1);
        }

        /// <summary>
        /// Bu metot bir nesneyi transaction işlemi ile güncellemenizi sağlar.
        /// This method updates an entity with transaction.
        /// </summary>
        [TestMethod]
        public void UpdateAnEntityWithTransaction()
        {
            // Connection'a erişmeniz gereken durumlarda bağlantıyı sizin açmanız gerekmektedir.           
            // In case you need to reference the connection object, you need to open and manage the connection.
            gooContext.OrmConfiguration.Connection.Open();

            using (var transaction = gooContext.OrmConfiguration.Connection.BeginTransaction())
            {
                // Transaction nesnesini orm'e kullanacağımızı belirtmeliyiz.
                // You must pass in the transaction object's reference.
                gooContext.OrmConfiguration.UseTransaction(transaction);

                Categories category = gooContext.Categories.FirstOrDefault();

                category.CategoryName = "Computer Update";
                category.Description = "Update test";

                gooContext.Categories.Update(category);

                int result = gooContext.SubmitChanges();

                if (result > -1)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        /// <summary>
        /// Bu metot bir nesne silmenizi sağlar.
        /// This method deletes an entity.
        /// </summary>
        [TestMethod]
        public void DeleteAnEntity()
        {
            Categories category = gooContext.Categories.FirstOrDefault();

            gooContext.Categories.Delete(category);

            int result = gooContext.SubmitChanges();

            Assert.AreNotEqual(result, -1);
        }
        #endregion
    }
}