using System.Data.Entity;

namespace FrameWork.Core.Data.Migrations
{
    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    /// <typeparam name="TContext">模型实体。</typeparam>
    /// <typeparam name="TMigrationsConfiguration">数据库迁移配置。</typeparam>
    public class DatabaseMigrator<TContext, TMigrationsConfiguration>
        where TContext : BaseDbContext, new()
        where TMigrationsConfiguration : GenericConfiguration<TContext>, new()
    {
        /// <summary>
        /// 迁移数据库。
        /// </summary>
        public static void MigrateDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration>());
            using (var dbContext = new TContext())
            {
                dbContext.Database.Initialize(true);
            }
        }
    }
}