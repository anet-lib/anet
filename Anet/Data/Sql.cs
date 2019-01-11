using System;
using System.Collections.Generic;
using System.Linq;

namespace Anet.Data
{
    public static class Sql
    {
        public static string Ands(params string[] paramNames)
        {
            return string.Join(" AND ", paramNames.Select(x => x + "=@" + x));
        }

        public static string Where(dynamic clause)
        {
            IEnumerable<string> paramNames = GetParamNames(clause as object);
            return Where(paramNames);
        }

        public static string Where(params string[] paramNames)
        {
            return "WHERE " + Ands(paramNames);
        }

        public static string Select(string tableName, dynamic clause)
        {
            return Select(tableName, GetParamNames(clause as object));
        }

        public static string Select(string tableName, params string[] clauseColumns)
        {
            var sql = $"SELECT * FROM {tableName} ";
            if (clauseColumns != null && clauseColumns.Count() > 0)
                sql += Where(clauseColumns);
            return sql;
        }

        public static string Insert(string table, dynamic values)
        {
            return Insert(table, GetParamNames(values as object));
        }

        public static string Insert(string table, params string[] columns)
        {
            Ensure.HasItems(columns, nameof(columns));

            return $"INSERT INTO {table}({string.Join(", ", columns)} VALUES(@{string.Join(", @", columns)})";
        }

        public static string Update(string table, dynamic update, dynamic clause)
        {
            return Update(table, GetParamNames(update as object), GetParamNames(clause as object));
        }

        public static string Update(string table, IEnumerable<string> updateColumns, IEnumerable<string> clauseColumns)
        {
            Ensure.HasItems(updateColumns, nameof(updateColumns));

            var sql = $"UPDATE {table} SET {string.Join(", ", updateColumns.Select(x => x + "=@" + x))} ";
            if (clauseColumns != null && clauseColumns.Count() > 0)
                sql += Where(clauseColumns);
            return sql;
        }

        public static string Delete(string table, dynamic clause)
        {
            return Delete(table, GetParamNames(clause as object));
        }

        public static string Delete(string table, IEnumerable<string> clauseColumns)
        {
            var sql = $"DELETE FROM {table} ";
            if (clauseColumns != null && clauseColumns.Count() > 0)
                sql += Where(clauseColumns);
            return sql;
        }

        /// <summary>
        /// Get parameter names from an object or <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="param">The param object to get properties.</param>
        /// <returns>The parameter names collection.</returns>
        public static IEnumerable<string> GetParamNames(object param)
        {
            if (param == null)
                return new HashSet<string>();

            if (param is DynamicParameters dynamicParameters)
                return dynamicParameters.ParameterNames;

            return param.GetType()
                .GetProperties().Where(x => x.PropertyType.IsSimpleType()).Select(x => x.Name);
        }

        /// <summary>
        /// Merge parameters.
        /// </summary>
        /// <param name="firstParam">The first parameter to merge.</param>
        /// <param name="otherParams">The other prameters to merge.</param>
        /// <returns>Merged parameters.</returns>
        public static DynamicParameters MergeParams(object firstParam, params object[] otherParams)
        {
            Ensure.NotNull(firstParam, nameof(firstParam));

            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(firstParam);
            foreach (var param in otherParams)
            {
                parameters.AddDynamicParams(param);
            }
            return parameters;
        }
    }
}
