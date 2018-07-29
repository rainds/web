using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace FrameWork.DataService
{
    /// <summary>
    /// 输出Sql执行信息
    /// </summary>
    internal class EfIntercepterLogging : DbCommandInterceptor
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public override void ScalarExecuting(DbCommand command,
            DbCommandInterceptionContext<object> interceptionContext)
        {
            this.stopwatch.Restart();
            base.ScalarExecuting(command, interceptionContext);
            
        }

        public override void ScalarExecuted(DbCommand command,
            DbCommandInterceptionContext<object> interceptionContext)
        {
            
            base.ScalarExecuted(command, interceptionContext);
            this.SetTraceInfo(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command,
            DbCommandInterceptionContext<int> interceptionContext)
        {
            this.stopwatch.Restart();
            base.NonQueryExecuting(command, interceptionContext);
            
        }

        public override void NonQueryExecuted(DbCommand command,
            DbCommandInterceptionContext<int> interceptionContext)
        {
            
            base.NonQueryExecuted(command, interceptionContext);
            this.SetTraceInfo(command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command,
            DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.stopwatch.Restart();
            base.ReaderExecuting(command, interceptionContext);
           
        }

        public override void ReaderExecuted(DbCommand command,
            DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            
            base.ReaderExecuted(command, interceptionContext);
            this.SetTraceInfo(command, interceptionContext);
        }

        private void SetTraceInfo<T>(DbCommand command,
            DbCommandInterceptionContext<T> interceptionContext, [CallerMemberName] string actionName = "")
        {
            this.stopwatch.Stop();

            var builder = new StringBuilder(4096);
            builder.Append("动作：").AppendLine(actionName);
            builder.AppendLine("语句：").AppendLine(command.CommandText);

            builder.AppendLine("参数：");
            foreach (DbParameter para in command.Parameters)
            {
                builder.AppendFormat("name={0}    value={1}", para.ParameterName, para.Value).AppendLine();
            }

            if (interceptionContext.Exception != null)
            {
                builder.AppendLine("错误信息：").Append(interceptionContext.Exception);
                Trace.TraceError(builder.ToString());
            }
            else
            {
                builder.AppendFormat("执行时间：{0} 毫秒", this.stopwatch.ElapsedMilliseconds);
                Trace.TraceInformation(builder.ToString());
            }
        }
    }
}