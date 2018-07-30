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
        public static void MigrateDatabase(string connectString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EfDbContext, GenericConfiguration>());

            using (var dbContext = new EfDbContext(connectString))
            {
                dbContext.Database.Initialize(true);
            }
        }
    }
}