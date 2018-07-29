using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Text;
using FrameWork.Core;
using FrameWork.Core.Data;

namespace FrameWork.DataService
{
    public class EfDbProvider : IDbProvider
    {
        private EfDbContext context;
        private DbContextTransaction transaction;
        private IDictionary<string, object> repositories;

        public EfDbProvider(EfDbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<string, object>();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class,new()
        {
            object obj;
            var typename = typeof(TEntity).FullName;

            if (this.repositories.TryGetValue(typename, out obj)) return (EfRepository<TEntity>)obj;

            var repository = new EfRepository<TEntity>(this.context);

            this.repositories.Add(typename, repository);

            return repository;
        }

        public void Begin()
        {
            if (this.context.HaveTransaction) return;

            this.transaction = this.context.Database.BeginTransaction();

            this.context.HaveTransaction = true;
        }

        public void Commit()
        {
            if (this.transaction == null) return;

            this.transaction.Commit();
            this.transaction = null;

            this.context.HaveTransaction = false;
        }

        public void Rollback()
        {
            if (this.transaction == null) return;

            this.transaction.Rollback();
            this.transaction = null;

            this.context.HaveTransaction = false;
        }

        public void Dispose()
        {
            if (this.transaction != null)
            {
                this.Rollback();

                var builder = new StringBuilder(4096);
                builder.Append("发现未处理的数据库事务,调用堆栈：");
                var stacks = new StackTrace().GetFrames();
                if (stacks != null)
                {
                    for (var index = 1; index < stacks.Length; index++)
                    {
                        var method = stacks[index].GetMethod();
                        if (method.DeclaringType == null)
                        {
                            builder.AppendFormat("\r\n[{0}]: {1}", index, method.Name);
                        }
                        else
                        {
                            builder.AppendFormat("\r\n[{0}]: {1}.{2}", index, method.DeclaringType.FullName, method.Name);
                        }
                    }
                }

                FrameWorkLogger.Log(builder.ToString(), "UnhandledTran");
            }

            foreach (var obj in this.repositories.Values)
            {
                ((IDisposable)obj).Dispose();
            }
            this.repositories.Clear();
            this.repositories = null;

            this.context.CloseConnection();
            this.context = null;
        }
    }
}