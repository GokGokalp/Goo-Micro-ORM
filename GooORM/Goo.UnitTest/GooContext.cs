  using Goo.OrmCore;
using Goo.UnitTest.Entities;

namespace Goo.UnitTest
{
public partial class GooContext : DBContextBase
	{
 
        public DBProviderBase<Categories> Categories
        {
            get
            {
                return DBProviderFactory<Categories>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Customers> Customers
        {
            get
            {
                return DBProviderFactory<Customers>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Orders> Orders
        {
            get
            {
                return DBProviderFactory<Orders>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Shippers> Shippers
        {
            get
            {
                return DBProviderFactory<Shippers>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Suppliers> Suppliers
        {
            get
            {
                return DBProviderFactory<Suppliers>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Employees> Employees
        {
            get
            {
                return DBProviderFactory<Employees>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Region> Region
        {
            get
            {
                return DBProviderFactory<Region>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Products> Products
        {
            get
            {
                return DBProviderFactory<Products>.GetDbProvider(base.OrmConfiguration);
            }
        }
        public DBProviderBase<Territories> Territories
        {
            get
            {
                return DBProviderFactory<Territories>.GetDbProvider(base.OrmConfiguration);
            }
        }
	}
}