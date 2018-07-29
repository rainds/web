﻿using System.Data.Entity.Migrations;
using System.Linq;

namespace FrameWork.DataService
{
    /// <summary>
    /// 数据库初始化配置。
    /// </summary>
    /// <typeparam name="TContext">数据库上下文对象。</typeparam>
    public class GenericConfiguration : DbMigrationsConfiguration<EfDbContext>
    {
        /// <summary>
        /// 数据库是否有迁移。
        /// </summary>
        private readonly bool isDatabaseMigration;

        /// <summary>
        /// 数据库初始化配置构造方法。
        /// </summary>
        public GenericConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            var migrator = new DbMigrator(this);
            this.isDatabaseMigration = migrator.GetDatabaseMigrations().Any();
        }

        /// <summary>
        /// 只在程序启动时初始化数据。
        /// </summary>
        /// <param name="context">数据库上下文实体对象。</param>
        protected virtual void OnSeed(EfDbContext context)
        {
        }

        /// <summary>
        /// 数据库初始化数据。
        /// </summary>
        /// <param name="context">数据库上下文实体对象。</param>
        protected override void Seed(EfDbContext context)
        {
            if (this.isDatabaseMigration)
            {
                return;
            }

            this.OnSeed(context);
        }
    }
}