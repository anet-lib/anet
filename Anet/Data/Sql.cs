using System;
using System.Collections.Generic;
using System.Linq;

namespace Anet.Data
{
    public static class Sql
    {
        public static string Like(string keyword)
        {
            return $"%{keyword.Replace(" ", "%")}%";
        }

        public static string And(object clause)
        {
            var names = GetParamNames(clause);
            return string.Join(" AND ", names.Select(x => x + "=@" + x));
        }

        public static string Where(object clause)
        {
            return "WHERE " + And(clause);
        }

        public static string Select(string table, object clause)
        {
            var sql = $"SELECT * FROM {table} ";
            if (clause != null)
                sql += Where(clause);
            return sql;
        }

        public static string Insert(string table, object columns)
        {
            var colNames = GetParamNames(columns);
            return $"INSERT INTO {table}({string.Join(", ", colNames)}) VALUES(@{string.Join(", @", colNames)})";
        }

        public static string Update(string table, object update, object clause)
        {
            var updateCols = GetParamNames(update);
            var clauseCols = GetParamNames(clause);
            var sql = $"UPDATE {table} SET {string.Join(", ", updateCols.Select(x => x + "=@" + x))} ";
            if (clauseCols != null && clauseCols.Count() > 0)
                sql += Where(clauseCols);
            return sql;
        }

        public static string Delete(string table, object clause)
        {
            var clauseCols = GetParamNames(clause);
            var sql = $"DELETE FROM {table} ";
            if (clauseCols != null && clauseCols.Count() > 0)
                sql += Where(clauseCols);
            return sql;
        }

        /// <summary>
        /// Get parameter names from an object or <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="param">Columns(array or string with comma separated), DynamicParameters, Dynamic object or Entity type.</param>
        /// <returns>The parameter names collection.</returns>
        public static IEnumerable<string> GetParamNames(object param)
        {
            if (param == null)
            {
                return new HashSet<string>();
            }

            if (param is string str)
            {
                if (str.Contains(','))
                    return str
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim());
                return new[] { str };
            }

            if (param is IEnumerable<string> array)
            {
                return array;
            }

            if (param is DynamicParameters dynamicParameters)
            {
                return dynamicParameters.ParameterNames;
            }

            var type = param is Type ? (param as Type) : param.GetType();

            return type
                .GetProperties()
                .Where(x => x.PropertyType.IsSimpleType())
                .Select(x => x.Name);
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
