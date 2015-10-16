using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Goo.UnitTest.Entities;
using System;
using Goo.CacheManagement;

namespace Goo.UnitTest.Tests
{
    /// <summary>
    /// GooContext sınıfı üzerinden listeleme, getirme ve önbelleğe ekleme gibi işlemleri yapabilirsiniz.
    /// You can use GooContext class to perform list, retrieve and cache operations.
    /// </summary>
    [TestClass]
    public class ReadTest
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

        #region Read Test Methods
        /// <summary>
        /// Bu metot nesneler içindeki ilk nesneyi getirmeye yarar. Herhangi bir durumda geriye null değer döner.
        /// This method fetches the first row of the entity. Otherwise returns null.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault()
        {
            Orders order = gooContext.Orders.FirstOrDefault();

            Assert.IsNotNull(order);
        }

        /// <summary>
        /// Bu metot tüm nesneleri listelemeye yarar. Herhangi bir durumda count 0 döner.
        /// This method returns the entity as a list. Otherwise returns 0.
        /// </summary>
        [TestMethod]
        public void ToList()
        {
            List<Orders> orders = gooContext.Orders.ToList();

            Assert.IsTrue(orders.Count > 0);
        }

        /// <summary>
        /// Bu metot lambda ifadeleri aracılığı ile istediğiniz koşuldaki entityleri getirmenizi sağlar.
        /// This method uses lambda expressions to let you write your own queries.
        /// </summary>
        [TestMethod]
        public void Where()
        {
            List<Orders> orders = gooContext.Orders.Where(o => o.ShipName.Contains("al")).ToList();

            Assert.IsTrue(orders.Count > 0);
        }

        /// <summary>
        /// Bu metotlar sıralama işlemlerini ve istediğiniz miktarda entity getirme işlemleri yapabilmenizi sağlar.
        /// This method performs ordering and limiting of the data returned by the entity.
        /// </summary>
        [TestMethod]
        public void FluentOrderAndTakeConditions()
        {
            List<Orders> orders = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").OrderByAscending(x => x.OrderID).Take(5).ToList();

            Assert.IsTrue(orders.Count > 0);
        }

        /// <summary>
        /// Bu metot kendi kompleks sorgunuzu oluşturabilmenizi sağlar.
        /// This method can be used to perform complex queries.
        /// </summary>
        [TestMethod]
        public void CustomInlineQuery()
        {
            var orders = gooContext.ExecuteCustomQuery("SELECT * FROM Orders WHERE ShipName LIKE '%al%'");

            Assert.IsTrue(orders.HasRows);
        }

        /// <summary>
        /// Bu metot kendi kompleks sorgunuzu parametreler ile oluşturabilmenizi sağlar.
        /// This method can be used to perform complex queries with parameters.
        /// </summary>
        [TestMethod]
        public void CustomInlineQueryWithParameters()
        {
            var orders = gooContext.ExecuteCustomQuery(@"SELECT * FROM Orders
                                                         WHERE ShipCountry = @ShipCountry AND ShipVia = @ShipVia", new { ShipCountry = "Brazil", ShipVia = "3" });

            Assert.IsTrue(orders.HasRows);
        }

        /// <summary>
        /// Bu metot kendi kompleks sorgularınızı Stored Procedure Table Direct veya Text olarak Insert,Update,Delete gibi işlemlere destekli sekilde parametresiz olarak oluşturmanızı sağlar.
        /// </summary>
        [TestMethod]
        public void CustomInlineNonQuery()
        {
            var act = gooContext.ExecuteCustomNonQuery(query: "Insert Into Categories (CategoryName,[Description]) Values ('Example','Example-Description')", commandType: System.Data.CommandType.Text);
            var expected = act > 0;

            Assert.IsTrue(expected);
        }

        /// <summary>
        /// Bu metot kendi kompleks sorgularınızı Stored Procedure Table Direct veya Text olarak Insert,Update,Delete gibi işlemlere destekli sekilde parametreli olarak oluşturmanızı sağlar.
        /// </summary>
        [TestMethod]
        public void CustomInlineNonQueryWithParameter()
        {
            var sqlParameters = new System.Data.SqlClient.SqlParameter[2];
            {
                sqlParameters[0] = new System.Data.SqlClient.SqlParameter("@catName", "Example");
                sqlParameters[1] = new System.Data.SqlClient.SqlParameter("@description", "Example-Description");
            }

            var act = gooContext.ExecuteCustomNonQuery(query: "Insert Into Categories (CategoryName,[Description]) Values ('@catName','@description')", commandType: System.Data.CommandType.Text, sqlParameters: sqlParameters);
            var expected = act > 0;

            Assert.IsTrue(expected);
        }

        /// <summary>
        /// Bu metot kendi kompleks sorgularınızı Stored Procedure olarak Insert,Update,Delete gibi işlemlere destekli sekilde parametreli olarak oluşturmanızı sağlar.
        /// </summary>
        /// 


        ///Bu testi geçebilmeniz için aşağıdaki basit SP örneğini local server ' da execute etmeniz gerekmektedir.
        ///


        //USE [GooNorthwind]
        //GO
        //SET ANSI_NULLS ON
        //GO
        //SET QUOTED_IDENTIFIER ON
        //GO
        //ALTER Procedure [dbo].[Insert_Categories](
        //    @catName NVARCHAR(15),
        //    @desc NTEXT
        //)
        //As
        //Begin
        //Insert Into Categories (CategoryName,[Description]) Values (
        //    @catName,
        //    @desc
        //)
        //End
        [TestMethod]
        public void CustomInlineNonQueryStoredProcedureWithParameter()
        {
            var sqlParameters = new System.Data.SqlClient.SqlParameter[2];
            {
                sqlParameters[0] = new System.Data.SqlClient.SqlParameter("@catName", "Example");
                sqlParameters[1] = new System.Data.SqlClient.SqlParameter("@desc", "Example-Description");
            }

            var act = gooContext.ExecuteCustomNonQuery(query: "Insert_Categories", commandType: System.Data.CommandType.StoredProcedure, sqlParameters: sqlParameters);
            var expected = act > 0;

            Assert.IsTrue(expected);
        }

        /// <summary>
        /// AddToCache ve GetFromCache metotları size in memory olarak nesneyi önbellekte tutma ve getirme işlemleri yapabilmenizi sağlar.
        /// The AddToCache and GetFromCache methods perform caching operations.
        /// </summary>
        [TestMethod]
        public void AddToCacheAndGetFromCacheConditions()
        {
            // Nesneyi önbelleğe varsayılan olarak limitsiz eklemeyi sağlar.
            // Adds the entity to the cache (non-expiry as default)
            List<Orders> ordersUntimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheUntimed").ToList();

            // Nesneyi önbelleğe belirlenen bir tarih boyunca eklemeyi sağlar.
            // Adds the entity to the cache (Timed)
            List<Orders> ordersTimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheTimed", CacheManager.EExpirationType.Expiration, new DateTime(2015, 6, 10)).ToList();

            // Nesneyi önbelleğe belirlenen bir süre boyunca eklemeyi sağlar.
            // Adds the entity to the cache (Sliding timed)
            List<Orders> ordersSlidingTimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheSlidingTimed", CacheManager.EExpirationType.SlidingExpiration, new TimeSpan(1, 0, 0)).ToList();

            // GetFromCache kullanarak daha önceden önbelleğe eklemiş olduğunuz nesneye erişmenizi sağlar. Nesne bulunamaması durumunda geriye null dönmektedir.
            // Allows you to get the objects from the cache. If no objects are found, returns null.
            List<Orders> ordersGetFromCacheUntimed = gooContext.Orders.GetFromCache("AddToCacheUntimed"); // "AddToCacheTimed" or "AddToCacheSlidingTimed"

            Assert.IsNotNull(ordersGetFromCacheUntimed);
        }

        /// <summary>
        /// OrmConfiguration özelliği size "LazyLoading, Connection ve Transaction" işlemleri yapabilmenizi sağlar.
        /// OrmConfiguration property will allow you to perform LazyLoading and manage Connection and Transaction operations
        /// </summary>
        [TestMethod]
        public void OrmConfiguration()
        {
            #region LazyLoading
            // Varsayılan olarak false dur.
            // default as false.
            gooContext.OrmConfiguration.LazyLoadingEnabled = true;

            Orders order = gooContext.Orders.FirstOrDefault();

            Assert.IsNotNull(order.Customer);
            #endregion

            #region Connection
            // Connection'a erişmeniz gereken durumlarda bağlantıyı sizin açmanız gerekmektedir.           
            // In case you need to reference the connection object, you need to open and manage the connection.
            var connection = gooContext.OrmConfiguration.Connection; // .Open();

            Assert.IsNotNull(connection);
            #endregion

            #region Transaction
            using (var transaction = gooContext.OrmConfiguration.Connection.BeginTransaction())
            {
                // Transaction nesnesini orm'e kullanacağımızı belirtmeliyiz.
                // You must pass in the transaction object's reference.
                gooContext.OrmConfiguration.UseTransaction(transaction);

                Categories category = gooContext.Categories.FirstOrDefault();

                category.Description = string.Format("{0} Updated", category.Description);

                gooContext.Categories.Update(category);

                int result = gooContext.SubmitChanges();

                if (result > -1)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
            #endregion
        }
        #endregion
    }
}