using Dapper;
using System.Data;

namespace Anet.Data;

public static class DbExtensionsAsync
{
    /// <summary>
    /// Execute a query asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static Task<IEnumerable<dynamic>> QueryAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static Task<IEnumerable<T>> QueryAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QueryAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<T> FirstAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirstAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<T> FirstOrDefaultAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirstOrDefaultAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<T> SingleAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QuerySingleAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<T> SingleOrDefaultAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QuerySingleOrDefaultAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<dynamic> FirstAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QueryFirstAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<dynamic> FirstOrDefaultAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QueryFirstOrDefaultAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<dynamic> SingleAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QuerySingleAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    public static Task<dynamic> SingleOrDefaultAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QuerySingleOrDefaultAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute a command asynchronously using Task.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The number of rows affected.</returns>
    public static Task<int> ExecuteAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform an asynchronous multi-mapping query with 2 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map"/>.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
    public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this Db db, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
        db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform an asynchronous multi-mapping query with 3 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map"/>.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
    public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this Db db, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
        db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform an asynchronous multi-mapping query with an arbitrary number of input types.
    /// This returns a single type, combined from the raw types via <paramref name="map"/>.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="types">Array of types in the recordset.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
    public static Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this Db db, string sql, Type[] types, Func<object[], TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
        db.Connection.QueryAsync(sql, types, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
    /// </summary>
    /// <param name="db">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
    /// or <see cref="T:DataSet"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// DataTable table = new DataTable("MyTable");
    /// using (var reader = ExecuteReader(db, sql, param))
    /// {
    ///     table.Load(reader);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public static Task<IDataReader> ExecuteReaderAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteReaderAsync(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="db">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <see cref="object"/>.</returns>
    public static Task<object> ExecuteScalarAsync(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteScalarAsync<object>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="db">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
    public static Task<T> ExecuteScalarAsync<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteScalarAsync<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);
}
