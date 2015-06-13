using System;

namespace Goo.OrmCore
{
    public class DBInitializerManager
    {
        #region Constructor
        private OrmConfiguration _ormConfiguration;
        private static readonly Lazy<DBInitializerManager> instance = new Lazy<DBInitializerManager>(() => new DBInitializerManager());
        public static DBInitializerManager getInstance
        {
            get
            {
                return instance.Value;
            }
        }
        private DBInitializerManager()
        {
            _ormConfiguration = new OrmConfiguration();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Eğer nesneniz ModelBase soyut sınıfından kalıtım alıyor ise bu metot veritabanınızı oluşturmayı sağlar.
        /// This method initializes the database, only if the entities inherit the ModelBase abstract class.
        /// </summary>
        /// <typeparam name="TContext">Entity context class.</typeparam>
        /// <returns></returns>
        public void InitializeDatabase<TContext>()
        {
            dynamic provider = DBProviderFactory<TContext>.GetDbProvider(_ormConfiguration);

            provider.CreateTables();
        }

        /// <summary>
        /// Eğer nesneniz ModelBase soyut sınıfından kalıtım alıyor ise bu metot tabloyu create veya alter edebilmenizi sağlar.
        /// This method runs the create/alter table, only if the entity inherit the ModelBase abstract class.
        /// </summary>
        /// <typeparam name="TEntity">Entity class.</typeparam>
        /// <returns></returns>
        public void CreateOrAlterTable<TEntity>()
        {
            dynamic provider = DBProviderFactory<TEntity>.GetDbProvider(_ormConfiguration);

            provider.CreateOrAlterTable();
        }

        /// <summary>
        /// Bu metot tabloyu drop edebilmenizi sağlar.
        /// This method drops the table.
        /// </summary>
        /// <typeparam name="TEntity">Entity class.</typeparam>
        /// <param name="forceDrop">If forceDrop=true drop table process will be forced.</param>
        public void DropTable<TEntity>(bool forceDrop = false)
        {
            dynamic provider = DBProviderFactory<TEntity>.GetDbProvider(_ormConfiguration);

            provider.DropTable(forceDrop);
        }

        /// <summary>
        /// Bu metot tabloyu truncate edebilmenizi sağlar.
        /// This method truncates table.
        /// </summary>
        /// <typeparam name="TEntity">Entity class.</typeparam>
        /// <param name="forceTruncate">If forceTruncate=true truncate table operation will be forced.</param>
        /// <returns></returns>
        public void TruncateTable<TEntity>(bool forceTruncate = false)
        {
            dynamic provider = DBProviderFactory<TEntity>.GetDbProvider(_ormConfiguration);

            provider.TruncateTable(forceTruncate);
        }
        #endregion
    }
}