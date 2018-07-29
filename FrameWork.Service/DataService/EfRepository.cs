using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FrameWork.Core.Data;

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
            return entities.Sum(entity => this.Insert(entity));
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
            var myPredicate = new OperationsVisitor().Modify(predicate);
            var command = DataCommon.CreateCommand((ObjectQuery)this.ObjectSet.Where(myPredicate));
            var traceSql = command.CommandText;
            var bulider = new StringBuilder(4096);
            var alias = this.context.NamePrefix + "Extent1" + this.context.NameSuffix;

            bulider.Append("DELETE ")
                .Append(traceSql.Substring(traceSql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase)));
            bulider.Replace(alias + ".", "").Replace("AS " + alias, "");

            var count = command.Parameters.Count;
            var parameters = new object[count];
            for (var index = 0; index < count; index++)
            {
                parameters[index] = command.Parameters[index];
            }

            return this.context.Database.ExecuteSqlCommand(bulider.ToString(), parameters);
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

        public int Update(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity>> updatePredicate)
        {
            var myPredicate = new OperationsVisitor().Modify(wherePredicate);
            var command = DataCommon.CreateCommand((ObjectQuery)this.ObjectSet.Where(myPredicate));

            //获取Update的赋值语句
            var count = command.Parameters.Count;
            var parameters = new ArrayList(count * 2);
            for (var index = 0; index < count; index++)
            {
                parameters.Add(command.Parameters[index]);
            }
            var valueObj = updatePredicate.Compile().Invoke();
            var updateMemberExpr = (updatePredicate.Body as MemberInitExpression);
            if (updateMemberExpr == null)
            {
                throw new InvalidOperationException("不支持的表达式语法：" + updatePredicate);
            }
            var paraBuilder = new StringBuilder(2048);
            var valueType = typeof(TEntity);

            var paramindex = 1;
            foreach (var name in updateMemberExpr.Bindings.Cast<MemberAssignment>()
                .Select(bind => bind.Member.Name))
            {
                var para = this.context.Factory.CreateParameter();
                if (para != null)
                {
                    var property = valueType.GetProperty(name);
                    para.ParameterName = string.Format("{0}up{1}", this.context.ParaPrefix, paramindex);
                    para.Value = property.GetValue(valueObj);
                    parameters.Add(para);

                    paraBuilder.AppendFormat("{3}{0}{4}={2}up{1},", DataCommon.GetColumnName(property, name),
                        paramindex, this.context.ParaPrefix, this.context.NamePrefix, this.context.NameSuffix);
                }

                paramindex++;
            }

            if (paraBuilder.Length == 0) throw new ArgumentException("没有需要更新的值");
            paraBuilder.Remove(paraBuilder.Length - 1, 1);

            var traceSql = command.CommandText;
            var bulider = new StringBuilder(4096);
            var tablename = this.context.NamePrefix + DataCommon.GetTableName(this.entityType) + this.context.NameSuffix;
            var alias = this.context.NamePrefix + "Extent1" + this.context.NameSuffix;

            bulider.Append("UPDATE ").Append(tablename).Append(" SET ");
            bulider.Append(paraBuilder).Append(" ");
            bulider.Append(traceSql.Substring(traceSql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase))
                .Replace(alias + ".", ""));

            return this.context.Database.ExecuteSqlCommand(bulider.ToString(), parameters.ToArray());
        }

        public int Update(Expression<Func<TEntity, bool>> wherePredicate,
            Expression<Func<TEntity, bool>> updatePredicate)
        {
            //处理赋值语句
            var myPredicate1 = new OperationsVisitor().Modify(updatePredicate);
            var updateCommand = DataCommon.CreateCommand((ObjectQuery)this.ObjectSet.Where(myPredicate1));
            var updateSql = updateCommand.CommandText;
            var paraBuilder = new StringBuilder(4096);
            var alias = this.context.NamePrefix + "Extent1" + this.context.NameSuffix;

            paraBuilder.Append(updateSql.Substring(updateSql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) + 5)
                .Replace(alias + ".", "").Replace("AND", ","));
            var subsqls = paraBuilder.ToString().Split(',');
            for (var index = 0; index < subsqls.Length; index++)
            {
                var str = subsqls[index].Trim(' ');
                if (str.StartsWith("(")) //如果要更新的字段外面有括号，则去除两端的括号
                {
                    subsqls[index] = str.Substring(1, str.Length - 2);
                }
            }
            var paraSql = string.Join(" , ", subsqls);

            //处理查询语句
            var myPredicate2 = new OperationsVisitor().Modify(wherePredicate);
            var selectCommand = DataCommon.CreateCommand((ObjectQuery)this.ObjectSet.Where(myPredicate2));
            var count = selectCommand.Parameters.Count;
            var parameters = new ArrayList(count);
            for (var index = 0; index < count; index++)
            {
                parameters.Add(selectCommand.Parameters[index]);
            }
            var traceSql = selectCommand.CommandText;
            var bulider = new StringBuilder(4096);
            var tablename = this.context.NamePrefix + DataCommon.GetTableName(this.entityType) + this.context.NameSuffix;

            bulider.Append("UPDATE ").Append(tablename).Append(" SET ");
            bulider.Append(paraSql).Append(" ");
            bulider.Append(traceSql.Substring(traceSql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase))
                .Replace(alias + ".", ""));

            return this.context.Database.ExecuteSqlCommand(bulider.ToString(), parameters.ToArray());
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