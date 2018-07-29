using System.Data.Entity;

namespace FrameWork.DataService
{
    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class DatabaseMigrator
    {
        /// <summary>
        /// 迁移数据库。
        /// </summary>
        public static void MigrateDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EfDbContext, GenericConfiguration>());

            //using (var dbContext = new EfDbContext())
            //{
            //    dbContext.Database.Initialize(true);
            //}
        }
    }
}