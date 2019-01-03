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

        public static string Where(params string[] paramNames)
        {
            return "WHERE " + Ands(paramNames);
        }

        public static string Where(object clauseParam)
        {
            IEnumerable<string> paramNames = GetParamNames(clauseParam);

            return Where(paramNames);
        }

        public static string Select(string tableName, object clauseParam)
        {
            return Select(tableName, GetParamNames(clauseParam));
        }

        public static string Select(string tableName, params string[] clauseColumns)
        {
            var sql = $"SELECT * FROM {tableName} ";
            if (clauseColumns != null || clauseColumns.Count() > 0)
                sql += Where(clauseColumns);
            return sql;
        }

        public static string Insert(string tableName, object param)
        {
            return Insert(tableName, GetParamNames(param));
        }

        public static string Insert(string tableName, params string[] columns)
        {
            Ensure.HaveItems(columns, nameof(columns));

            return $"INSERT INTO {tableName}({string.Join(", ", columns)} VALUES(@{string.Join(", @", columns)})";
        }

        public static string Update(string tableName, object updateParam, object clauseParam)
        {
            Ensure.NotNull(updateParam, nameof(updateParam));

            return Update(tableName, GetParamNames(updateParam), GetParamNames(clauseParam));
        }

        public static string Update(string tableName, IEnumerable<string> updateColumns, IEnumerable<string> clauseColumns)
        {
            Ensure.HaveItems(updateColumns, nameof(updateColumns));

            var sql = $"UPDATE {tableName} SET {string.Join(", ", updateColumns.Select(x => x + "=@" + x))} ";
            if (clauseColumns != null || clauseColumns.Count() > 0)
                sql += Where(clauseColumns);
            return sql;
        }

        public static string Delete(string tableName, object param)
        {
            return Delete(tableName, GetParamNames(param));
        }

        public static string Delete(string tableName, params string[] clauseColumns)
        {
            Ensure.HaveItems(clauseColumns, nameof(clauseColumns));

            return $"DELETE FROM {tableName} {Where(clauseColumns)}";
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

            return param.GetType().GetProperties().Select(x => x.Name);
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
