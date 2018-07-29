using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FrameWork.DataService
{
    /// <summary>
    /// 将复制表达式转换为简单的常量表达式
    /// </summary>
    public class OperationsVisitor : ExpressionVisitor
    {
        public Expression<T> Modify<T>(Expression<T> expression)
        {
            return (Expression<T>)this.Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var ce = ConvertMemberExpression(node);
            return ce ?? base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var ce = ConvertMethodCallExpression(node);
            return ce ?? base.VisitMethodCall(node);
        }

        //转换成员访问表达式的为常量表达式
        private static ConstantExpression ConvertMemberExpression(MemberExpression me)
        {
            if (me.Expression is MemberExpression)
            {
                var ce = ConvertMemberExpression(me.Expression as MemberExpression);
                return ConvertMemberExpression(me, ce);
            }

            if (me.Expression is ConstantExpression)
            {
                var ce = me.Expression as ConstantExpression;
                return ConvertMemberExpression(me, ce);
            }

            return null;
        }

        private static ConstantExpression ConvertMemberExpression(MemberExpression me, ConstantExpression ce)
        {
            var obj = ce.Value;

            if (me.Member is FieldInfo)
            {
                var fd = me.Member as FieldInfo;
                return Expression.Constant(fd.GetValue(obj));
            }

            if (me.Member is PropertyInfo)
            {
                var pf = me.Member as PropertyInfo;
                return Expression.Constant(pf.GetValue(obj, null));
            }

            return null;
        }

        //转换方法调用表达式的为常量表达式
        private static ConstantExpression ConvertMethodCallExpression(MethodCallExpression mce)
        {
            if (mce.Object is ConstantExpression)
            {
                object obj = (mce.Object as ConstantExpression).Value;
                return Expression.Constant(mce.Method.Invoke(obj, mce.Arguments.Cast<object>().ToArray()));
            }

            throw new InvalidOperationException("不支持的表达式语法：" + mce);
        }
    }
}