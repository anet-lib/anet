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

        public static string Where(object param)
        {
            Ensure.NotNull(param, nameof(param));

            IEnumerable<string> paramNames;

            if (param is DynamicParameters dynamicParameters)
                paramNames = dynamicParameters.ParameterNames;
            else
                paramNames = param.GetType().GetProperties().Select(x => x.Name);

            return Where(paramNames);
        }

        public static string Insert(string tableName, params string[] columns)
        {
            Ensure.HaveItems(columns, nameof(columns));

            return $"INSERT INTO {tableName}({string.Join(", ", columns)} VALUES(@{string.Join(", @", columns)});";
        }

        public static string Update(string tableName, IEnumerable<string> updateColumns, IEnumerable<string> clauseColumns)
        {
            Ensure.HaveItems(updateColumns, nameof(updateColumns));

            var sql = $"UPDATE {tableName} SET {string.Join(", ", updateColumns.Select(x => x + "=@" + x))} ";

            if (clauseColumns != null || clauseColumns.Count() > 0)
                sql += Where(clauseColumns);

            return sql;
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
