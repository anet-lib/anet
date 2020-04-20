using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Anet.Data
{
    public static class Sql
    {
        public static string Like(string keyword)
        {
            return $"%{keyword?.Replace(" ", "%")}%";
        }

        public static string Limit(int page, int size)
        {
            return $"LIMIT {size * (page - 1)},{size}";
        }

        public static string And(object clause)
        {
            if (clause == null) return "1=1";

            var names = GetParamNames(clause);
            var type = clause.GetType();

            return string.Join(" AND ", names.Select(x =>
            {
                var propertyValue = type.GetProperty(x).GetValue(clause);
                if (propertyValue == null) return x + " IS NULL";
                else if (propertyValue is IEnumerable && !(propertyValue is string))
                    return x + " IN @" + x;
                else return x + "=@" + x;
            }));
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

        /// <summary>
        /// 生成INSERT语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="columns">
        /// <para>
        /// 需要生成语句的列。参数可以是：
        /// 1. 用“,”分隔的多个列名字符串
        /// 2. 列名字符串数组
        /// 3. 包括列名的对象若其类型
        /// </para>
        /// </param>
        /// <returns></returns>
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
                sql += Where(clause);
            return sql;
        }

        public static string Delete(string table, object clause)
        {
            var clauseCols = GetParamNames(clause);
            var sql = $"DELETE FROM {table} ";
            if (clauseCols != null && clauseCols.Count() > 0)
                sql += Where(clause);
            return sql;
        }

        /// <summary>
        /// Get parameter names from an object or <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="param">Columns(array or string with comma separated), DynamicParameters, Dynamic object or Entity type.</param>
        /// <returns>The parameter names collection.</returns>
        public static string[] GetParamNames(object param)
        {
            if (param == null)
            {
                return new string[] { };
            }

            if (param is string str)
            {
                if (str.Contains(','))
                    return str
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim()).ToArray();
                return new[] { str };
            }

            if (param is IEnumerable<string> array)
            {
                return array.ToArray();
            }

            if (param is DynamicParameters dynamicParameters)
            {
                return dynamicParameters.ParameterNames.ToArray();
            }

            var type = param is Type ? (param as Type) : param.GetType();

            return type
                .GetProperties()
                .Where(x => x.PropertyType.IsSimpleType() || x.PropertyType.GetAnyElementType().IsSimpleType())
                .Select(x => x.Name).ToArray();
        }

        /// <summary>
        /// Merge parameters.
        /// </summary>
        /// <param name="firstParam">The first parameter to merge.</param>
        /// <param name="otherParams">The other prameters to merge.</param>
        /// <returns>Merged parameters.</returns>
        public static DynamicParameters MergeParams(object firstParam, params object[] otherParams)
        {
            Guard.NotNull(firstParam, nameof(firstParam));

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
