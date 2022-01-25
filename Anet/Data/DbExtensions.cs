using Dapper;
using System.Data;

namespace Anet.Data;

public static class DbExtensions
{
    /// <summary>
    /// Execute parameterized SQL.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The number of rows affected.</returns>
    public static int Execute(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.Execute(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="db">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell selected as <see cref="object"/>.</returns>
    public static object ExecuteScalar(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteScalar(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="db">The connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
    public static T ExecuteScalar<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.ExecuteScalar<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

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
    public static IDataReader ExecuteReader(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.ExecuteReader(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Return a sequence of dynamic objects with properties matching the columns.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static IEnumerable<dynamic> Query(this Db db, string sql, object param = null, bool buffered = true, CommandType? commandType = null) =>
        db.Connection.Query(sql, param, db.Transaction, buffered, db.CommandTimeout, commandType);

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static dynamic First(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirst(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static dynamic FirstOrDefault(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirstOrDefault(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static dynamic Single(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QuerySingle(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public static dynamic SingleOrDefault(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QuerySingleOrDefault(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="buffered">Whether to buffer results in memory.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static IEnumerable<T> Query<T>(this Db db, string sql, object param = null, bool buffered = true, CommandType? commandType = null) =>
       db.Connection.Query<T>(sql, param, db.Transaction, buffered, db.CommandTimeout, commandType);

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T First<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirst<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T FirstOrDefault<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
        db.Connection.QueryFirstOrDefault<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T Single<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QuerySingle<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="db">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public static T SingleOrDefault<T>(this Db db, string sql, object param = null, CommandType? commandType = null) =>
       db.Connection.QuerySingleOrDefault<T>(sql, param, db.Transaction, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform a multi-mapping query with 2 input types.
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
    public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this Db db, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
          db.Connection.Query(sql, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform a multi-mapping query with 3 input types.
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
    public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this Db db, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
        db.Connection.Query(sql, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);

    /// <summary>
    /// Perform a multi-mapping query with an arbitrary number of input types.
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
    public static IEnumerable<TReturn> Query<TReturn>(this Db db, string sql, Type[] types, Func<object[], TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null) =>
        db.Connection.Query(sql, types, map, param, db.Transaction, buffered, splitOn, db.CommandTimeout, commandType);
}
