using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using FastReflection;

namespace FrameWork.DataService
{
    internal static class DataCommon
    {
        public static string GetTableName(Type entityType)
        {
            var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
            return tableAttr == null ? entityType.Name.Replace("Entity", "") : tableAttr.Name;
        }

        public static string GetColumnName(PropertyInfo property, string propertyName)
        {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttr == null ? propertyName : columnAttr.Name;
        }

        #region 调用EF内部方法，生成DbCommand

        private static PropertyInfo queryStateProperty;
        private static MethodInfo executionPlanMethod;
        private static FieldInfo definitionField;
        private static FieldInfo mappedField;

        /// <summary>
        /// 调用EF内部方法，生成DbCommand
        /// </summary>
        public static DbCommand CreateCommand(ObjectQuery query)
        {
            const BindingFlags Flag = BindingFlags.Instance | BindingFlags.NonPublic;
            if (queryStateProperty == null)
            {
                queryStateProperty = typeof(ObjectQuery).GetProperty("QueryState", Flag);
            }
            var objectQueryState = queryStateProperty.FastGetValue(query);

            if (executionPlanMethod == null)
            {
                executionPlanMethod = objectQueryState.GetType().GetMethod("GetExecutionPlan", Flag);
            }
            var executionPlan = executionPlanMethod.FastInvoke(objectQueryState, new object[] { null });

            if (definitionField == null)
            {
                definitionField = executionPlan.GetType().GetField("CommandDefinition", Flag);
                if (definitionField == null)
                    throw new InvalidOperationException("反射CommandDefinition属性失败");
            }
            var definition = definitionField.FastGetValue(executionPlan);

            if (mappedField == null)
            {
                mappedField = definition.GetType().GetField("_mappedCommandDefinitions", Flag);
                if (mappedField == null)
                    throw new InvalidOperationException("反射_mappedCommandDefinitions属性失败");
            }
            var list = (List<DbCommandDefinition>)mappedField.FastGetValue(definition);

            return list[0].CreateCommand();
        }

        #endregion 调用EF内部方法，生成DbCommand
    }
}