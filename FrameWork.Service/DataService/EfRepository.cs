using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using FrameWork.Core.Data;
using Z.EntityFramework.Plus;

namespace FrameWork.DataService
{
    public class EfRepository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, new()
    {
        private DbSet<TEntity> dbset;
        private EfDbContext context;
        private ObjectSet<TEntity> objectSet;
        private Type entityType;

        public ObjectSet<TEntity> ObjectSet
        {
            get
            {
                if (this.objectSet != null) return this.objectSet;

                var objContext = ((IObjectContextAdapter)this.context).ObjectContext;
                this.objectSet = objContext.CreateObjectSet<TEntity>();
                return this.objectSet;
            }
        }

        public EfRepository(EfDbContext context)
        {
            this.context = context;
            this.dbset = this.context.Set<TEntity>();
            this.entityType = typeof(TEntity);
        }

        private void Detach(TEntity entity)
        {
            var objectContext = ((IObjectContextAdapter)this.context).ObjectContext;
            try
            {
                objectContext.Detach(entity);
            }
            catch (InvalidOperationException)
            {
            }
        }

        public int Insert(TEntity entity)
        {
            try
            {
                this.dbset.Add(entity);
                var count = this.context.SaveChanges();
                return count;
            }
            finally
            {
                this.Detach(entity);
            }
        }

        public int Insert(IList<TEntity> entities)
        {
            this.dbset.AddRange(entities);
            return this.context.SaveChanges();
        }

        public int Delete(IList<TEntity> entities)
        {
            this.dbset.RemoveRange(entities);
            return this.context.SaveChanges();
        }

        public int Delete(TEntity entity)
        {
            this.dbset.Attach(entity);
            this.dbset.Remove(entity);
            try
            {
                try
                {
                    return this.context.SaveChanges();
                }
                catch (EntityException)
                {
                    return this.context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
            finally
            {
                this.Detach(entity);
            }
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbset.Where(predicate).Delete();
        }

        public int Update(TEntity entity)
        {
            try
            {
                this.context.Entry(entity).State = EntityState.Modified;
                try
                {
                    return this.context.SaveChanges();
                }
                catch (EntityException)
                {
                    return this.context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
            finally
            {
                this.Detach(entity);
            }
        }

        public int Update(Expression<Func<TEntity, TEntity>> updatePredicate)
        {
            return this.dbset.Update(updatePredicate);
        }

        public int Update(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TEntity>> updatePredicate)
        {
            return this.dbset.Where(wherePredicate).Update(updatePredicate);
        }

        public TEntity Find(object keyValue)
        {
            var entity = this.dbset.Find(keyValue);
            if (entity != null) this.Detach(entity);
            return entity;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = this.dbset.Where(predicate).FirstOrDefault();
            if (entity != null) this.Detach(entity);
            return entity;
        }

        public IQueryable<TEntity> Query
        {
            get { return this.dbset.AsNoTracking(); }
        }

        public void Dispose()
        {
            this.dbset = null;
            this.context = null;
            this.entityType = null;
        }
    }
}