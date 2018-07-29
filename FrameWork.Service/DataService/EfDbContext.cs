using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FrameWork.DataService
{
    public class EfDbContext : DbContext
    {
        private readonly DbConnection connection;

        /// <summary>
        /// 是否开启事务
        /// </summary>
        internal bool HaveTransaction { get; set; }

        /// <summary>
        /// 参数前缀
        /// </summary>
        internal string ParaPrefix { get; set; }

        /// <summary>
        /// 命名前缀
        /// </summary>
        internal string NamePrefix { get; set; }

        /// <summary>
        /// 命名后缀
        /// </summary>
        internal string NameSuffix { get; set; }

        /// <summary>
        /// 数据库对象工厂
        /// </summary>
        internal DbProviderFactory Factory { get; private set; }

        public EfDbContext(DbConnection connection, DbProviderFactory factory)
            : base(connection, true)
        {
            this.connection = connection;
            this.Factory = factory;

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        internal void CloseConnection()
        {
            if (!this.HaveTransaction && this.connection.State != System.Data.ConnectionState.Closed)
            {
                this.connection.Close();
            }
        }

        #region 注册实体

        //延迟注册的实体类型
        private static readonly IDictionary<string, Type> LazyEntityTypes = new Dictionary<string, Type>();

        /// <summary>
        /// 注册实体类型
        /// </summary>
        internal static void RegisterEntity(Type entityType)
        {
            if (!LazyEntityTypes.ContainsKey(entityType.FullName))
            {
                LazyEntityTypes.Add(entityType.FullName, entityType);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //移除级联删除的契约
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //移除表名复数的契约
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //注册实体类型
            foreach (var type in LazyEntityTypes.Values)
            {
                modelBuilder.RegisterEntityType(type);
            }

            //配置以Entity结尾的实体映射成不含Entity结尾的表
            modelBuilder.Types().Configure(type => type.ToTable(DataCommon.GetTableName(type.ClrType)));
            //配置所有的实体属性对应生成数据库字段
            modelBuilder.Properties().Configure(property => property.HasColumnName(DataCommon.GetColumnName(property.ClrPropertyInfo, property.ClrPropertyInfo.Name)));
        }

        #endregion 注册实体
    }
}