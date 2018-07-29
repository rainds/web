using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using FrameWork.Core;
using FrameWork.Core.Config;
using FrameWork.Core.Data;

namespace FrameWork.DataService
{
    public class EfDbFactory : IDbFactory, IDisposable
    {
        private IDictionary<string, EfDbContext> contexts = new Dictionary<string, EfDbContext>();

        public IDbProvider GetDbProvider(string dbkey)
        {
            EfDbContext context;
            if (this.contexts.TryGetValue(dbkey, out context)) return new EfDbProvider(context);

            DbConfigInfo info;
            var config = ConfigurationManager.ConnectionStrings[dbkey];
            if (config != null) //尝试读取配置文件
            {
                info = new DbConfigInfo
                {
                    ConnectionString = config.ConnectionString,
                    ProviderName = config.ProviderName
                };
            }
            else //尝试读取数据库配置
            {
                info = Locator.Get<IConfig>().Get<DbConfigInfo>();
            }

            if (info == null)
                throw new ArgumentException("数据库关键字" + dbkey + "的配置不存在");

            var factory = DbProviderFactories.GetFactory(info.ProviderName);
            if (factory == null)
                throw new ArgumentException("不支持的数据库提供者：" + info.ProviderName);

            var connection = factory.CreateConnection();
            if (connection == null)
                throw new ArgumentException("数据库提供者无法创建连接：" + info.ProviderName);
            connection.ConnectionString = info.ConnectionString;

            context = new EfDbContext(connection, factory);
            switch (info.ProviderName)
            {
                case "MySql.Data.MySqlClient":
                    context.ParaPrefix = "@";
                    context.NamePrefix = "`";
                    context.NameSuffix = "`";
                    break;

                default:
                    context.ParaPrefix = "@";
                    context.NamePrefix = "[";
                    context.NameSuffix = "]";
                    break;
            }

            this.contexts.Add(dbkey, context);
            return new EfDbProvider(context);
        }

        public void RegisterEntities(Assembly assembly)
        {
            var baseType = typeof(IEntity);
            foreach (var entityType in assembly.GetTypes().Where(m => baseType.IsAssignableFrom(m) && !m.IsAbstract))
            {
                EfDbContext.RegisterEntity(entityType);
            }
        }

        public void Dispose()
        {
            this.contexts.Clear();
            this.contexts = null;
        }
    }
}