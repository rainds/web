using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace FrameWork.Core.Data
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
    }
}