using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            context = new EfDbContext(dbkey);

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