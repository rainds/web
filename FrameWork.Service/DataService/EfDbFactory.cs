using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameWork.Core.Data;

namespace FrameWork.DataService
{
    public class EfDbFactory : IDbFactory, IDisposable
    {
        private IDictionary<string, BaseDbContext> contexts = new Dictionary<string, BaseDbContext>();

        public void RegisterEntities(Assembly assembly)
        {
            var baseType = typeof(IEntity);
            foreach (var entityType in assembly.GetTypes().Where(m => baseType.IsAssignableFrom(m) && !m.IsAbstract))
            {
                BaseDbContext.RegisterEntity(entityType);
            }
        }

        public void Dispose()
        {
            this.contexts.Clear();
            this.contexts = null;
        }

        public IDbProvider GetDbProvider<TContext>() where TContext : BaseDbContext, new()
        {
            BaseDbContext context;
            var dbkey = typeof(TContext).FullName;
            if (this.contexts.TryGetValue(dbkey, out context)) return new EfDbProvider(context);

            context = new TContext();

            this.contexts.Add(dbkey, context);
            return new EfDbProvider(context);
        }
    }
}