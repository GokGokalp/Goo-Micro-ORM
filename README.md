Goo Micro ORM .Net için geliştirilmiş typed ve non-typed destekli, code-first yaklaşımlı basit bir orm aracıdır.

# SUPPORTS
* Şuan sadece MSSQL veritabanını desteklemektedir.
* Veri modelinizi code-first yaklaşımı ile oluşturmanıza olanak sağlamaktadır.
* Basit tablo işlemlerini gerçekleştirebilmenizi sağlamaktadır.
* Listeleme işlemlerini type destekli bir şekilde yapabilmenizi sağlamaktadır.
* LINQ kullanımına olanak sağlamaktadır.
* Caching işlemlerini desteklemektedir.
* Transaction işlemlerini desteklemektedir.
* Custom kompleks query yazabilmeye olanak sağlamaktadır.

# USAGE
Goo Micro ORM'i projenize ekleyip ilgili connection ayarlarını tanımladıktan sonra kendi modelinizi oluşturmaya hemen başlayabilirsiniz.

`Typed desteğini kullanabilmeniz için:`
* Kendi entitylerinizi ModelBase soyut sınıfından türetin.

`Propertyleri veritabanı tarafında eşleyebilmek için:`
* Entitylerinizi ilgili type attributeleri ile işaretleyin.

Categories entitysi için örnek bir model tanımlaması:

```csharp
using Goo.Attributes;
using Goo.OrmCore;

namespace Goo.UnitTest.Entities
{
    public class Categories : ModelBase
    {
        [IsPrimaryKey]
        [IsAutoIncrement(1, 1)]
        public int CategoryID { get; set; }

        [NVARCHAR(15)]
        [NOTNULL]
        public string CategoryName { get; set; }

        [NTEXT]
        [NULL]
        public string Description { get; set; }

        [IMAGE]
        [NULL]
        public byte[] Picture { get; set; }
    }
}
```

`Desteklenen tipler: BOOLEAN, DATETIME, DECIMAL, IMAGE, INT, IsAutoIncrement, IsForeignKey, IsPrimaryKey, IsRelationEntity, MONEY, NCHAR, NONCLUSTEREDINDEX, NOTNULL, NTEXT, NULL, NVARCHAR, SMALLINT, TINYINT, VARCHAR`


Database initialization
-----------------------
`Entitylerinizi ModelBase soyut sınıfından türeterek oluşturduktan sonra projeyi derlediğinizde GooORM size GooContext wrapper sınıfını oluşturacaktır.`

```csharp
DBInitializerManager dbInitializerManager = DBInitializerManager.getInstance;

dbInitializerManager.InitializeDatabase<GooContext>();
```

Create/Alter/Drop/Truncate table operations
-------------------------------------------
`Tablo üzerindeki işlemlerinizi kolaylıkla DBInitializerManager üzerinden gerçekleştirebilirsiniz.`

```csharp
DBInitializerManager dbInitializerManager = DBInitializerManager.getInstance;

dbInitializerManager.DropTable<Categories>();
dbInitializerManager.CreateOrAlterTable<Categories>();
dbInitializerManager.TruncateTable<Categories>();
```

CRUD Operations
--------------
`Using the GooContext class you can perform basic operations like insert/update or delete an entity.`

`Insert an entity:`
```csharp
            GooContext gooContext = new GooContext();
            
            Categories category = new Categories()
            {
                CategoryName = "Computer",
                Description = "Insert test"
            };

            gooContext.Categories.Insert(category);

            int result = gooContext.SubmitChanges();
```
`Update an entity:`
```csharp
            GooContex gooContext = new GooContext();
            
            Categories category = gooContext.Categories.FirstOrDefault();

            category.CategoryName = "Computer Update";
            category.Description = "Update test";

            gooContext.Categories.Update(category);

            int result = gooContext.SubmitChanges();
```
`Delete an entity:`
```csharp
            GooContex gooContext = new GooContext();
            
            Categories category = gooContext.Categories.FirstOrDefault();

            gooContext.Categories.Delete(category);

            int result = gooContext.SubmitChanges();
```

Read Operations
---------------
`GooContext sınıfı üzerinden listeleme, getirme ve önbelleğe ekleme gibi işlemleri yapabilirsiniz.`

`FirstOrDefault:`
```csharp
            GooContext gooContext = new GooContext();
            
            Orders order = gooContext.Orders.Where(x=>x.RequiredDate == DateTime.Now).FirstOrDefault();
```
`ToList:`
```csharp
            GooContext gooContext = new GooContext();
            
            List<Orders> orders = gooContext.Orders.ToList();
```
`Where:`
```csharp
            GooContext gooContext = new GooContext();
            
            List<Orders> orders = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").ToList();
```
`Order and Take conditions:`
```csharp
            GooContext gooContext = new GooContext();
            
            List<Orders> orders = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").OrderByAscending(x => x.OrderID).Take(5).ToList();
```
`Custom inline query:`
```csharp
            GooContext gooContext = new GooContext();
            
            var orders = gooContext.ExecuteCustomQuery(@"SELECT * FROM Orders
                                                         WHERE ShipCountry = 'Brazil' AND ShipVia = 3");
```
`AddToCache and GetFromCache conditions:`
```csharp
            GooContext gooContext = new GooContext();
            
            // Adds the entity to the cache (non-expiry as default)
            List<Orders> ordersUntimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheUntimed").ToList();

            // Adds the entity to the cache (Timed)
            List<Orders> ordersTimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheTimed", CacheManager.EExpirationType.Expiration, new DateTime(2015, 6, 10)).ToList();

            // Adds the entity to the cache (Sliding timed)
            List<Orders> ordersSlidingTimed = gooContext.Orders.Where(o => o.OrderDate > DateTime.Parse("1997-12-31") && o.ShipCountry == "Brazil").AddToCache("AddToCacheSlidingTimed", CacheManager.EExpirationType.SlidingExpiration, new TimeSpan(1, 0, 0)).ToList();

            // Allows you to get the objects from the cache. If no objects are found, returns null.
            List<Orders> ordersGetFromCacheUntimed = gooContext.Orders.GetFromCache("AddToCacheUntimed"); // "AddToCacheTimed" or "AddToCacheSlidingTimed"
```

ORM Configuration
---------------
`OrmConfiguration property will allow you to perform LazyLoading and manage Connection and Transaction operations.`

`LazyLoading:`
```csharp
            GooContex gooContext = new GooContext();
            
            /// Lazy loading is enabled, related objects are loaded when they are accessed through a navigation property. (false as default)
            gooContext.OrmConfiguration.LazyLoadingEnabled = true;

            Orders order = gooContext.Orders.FirstOrDefault();
            order.Customer...?
```
`Connection:`
```csharp
            GooContex gooContext = new GooContext();
            
            // In case you need to reference the connection object, you need to open and manage the connection.
            var connection = gooContext.OrmConfiguration.Connection; // .Open();
```
`Transaction:`
```csharp
            GooContex gooContext = new GooContext();
            
            using (var transaction = gooContext.OrmConfiguration.Connection.BeginTransaction())
            {
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
```

# PERFORMANCE TESTS
`Performance of Select (strongly typed)`

<table>
  <tr>
  	<th>Method</th>
    <th>Duration</th>
    <th>Count</th>	
	</tr>
	<tr>
		<td><code>ToList()</code></td>
		<td>116ms</td>
		<td>500</td>
	</tr>
		<tr>
		<td><code>Where(o => o.ShipVia == 3 && o.RequiredDate > DateTime.Parse("1996-09-01"))</code></td>
		<td>130ms</td>
		<td>246</td>
	</tr>
	<tr>
		<td><code>Where(o => o.ShipName.Contains("al"))</code></td>
		<td>95ms</td>
		<td>87</td>
	</tr>
</table>

`Performance of Select (non-typed DBDataReader)`

<table>
  <tr>
  	<th>Method</th>
	<th>Duration</th>
		<th>Count</th>	
	</tr>
	<tr>
		<td><code>ExecuteCustomQuery</code></td>
		<td>76ms</td>
		<td>500</td>
	</tr>
		<tr>
		<td><code>ExecuteCustomQuery("SELECT * FROM Orders WHERE ShipVia = 3 AND RequiredDate > '1996-09-01'")</code></td>
		<td>80ms</td>
		<td>246</td>
	</tr>
		</tr>
		<tr>
		<td><code>ExecuteCustomQuery("SELECT * FROM Orders WHERE ShipName LIKE '%al%'")</code></td>
		<td>78ms</td>
		<td>87</td>
	</tr>
</table>
