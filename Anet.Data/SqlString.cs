using System.Text;

namespace Anet.Data;

public class SqlString
{
    readonly StringBuilder _sb;

    public SqlString() => _sb = new StringBuilder();
    public SqlString(string value) => _sb = new StringBuilder(value);
    public SqlString(DbDialect dialect) : this() => Dialect = dialect;

    public static implicit operator string(SqlString s) => s._sb.ToString();
    public static implicit operator SqlString(string s) => new(s);

    public DbDialect Dialect { get; init; }

    public SqlString Append(char c)
    {
        _sb.Append(c);
        return this;
    }
    public SqlString Append(string value)
    {
        _sb.Append(value);
        return this;
    }
    public SqlString Prepend(string value)
    {
        _sb.Insert(0, value);
        return this;
    }
    public SqlString Clear()
    {
        _sb.Clear();
        return this;
    }

    public SqlString AppendLine(string value = null) => Append(value).Append('\n');
    public SqlString AppendSpace(string value = null) => Append(value).Append(' ');
    public SqlString End() => Append(';');

    public SqlString Select(object obj, string prefix = null) =>
        Prepend("SELECT ").ForEachCol(obj, n => Column(n, prefix).Append(", ")).RemoveTrail(2);

    public SqlString From(string table, string alias = null)
    {
        Append("FROM ").Append(table);
        if (!string.IsNullOrEmpty(alias))
            Append(" AS ").Append(alias);
        return AppendLine();
    }

    public SqlString Update(string table) => Append("UPDATE ").AppendLine(table);

    public SqlString Set(object obj) => ForEachCol(obj, name => Opt(name).Append(", ")).RemoveTrail(2);

    public SqlString Insert(string table) => Append("INSERT INTO ").AppendSpace(table);

    public SqlString Values(object obj) =>
        Append('(').ForEachCol(obj, n => Append(n).Append(", ")).RemoveTrail(2).Append(") ")
        .Append("VALUES(").ForEachCol(obj, n => Append('@').Append(n).Append(", ")).RemoveTrail(2);

    public SqlString InsertedId()
    {
        return Dialect switch
        {
            DbDialect.MySQL => End().Select("LAST_INSERT_ID()"),
            DbDialect.SQLite => End().Select("last_insert_rowid()"),
            DbDialect.SQLServer => End().Select("SCOPE_IDENTITY()"),
            DbDialect.PostgreSQL => End().Select("LASTVAL()"),
            _ => throw new NotSupportedException("To call the InsertedId method, you need to set the DbType first."),
        };
    }

    public SqlString Delete(string table) => Append("DELETE FROME ").AppendSpace(table);

    public SqlString Where() => AppendLine().Append("WHERE 1=1 ");
    public SqlString Where(object obj, string prefix = null) => Where().And(obj, prefix);

    public SqlString And() => Append("\n\tAND ");
    public SqlString And(object obj, string prefix = null) => ForEachCol(obj, name => And().Opt(name, "=", prefix));
    public SqlString AndIn(string name, string prefix = null) => And().Opt(name, " IN ", prefix);
    public SqlString AndLike(string name, string prefix = null) => And().Opt(name, " LIKE ", prefix);
    public SqlString AndBetween(string name, string start, string end, string prefix = null) =>
        And().Column(name, prefix).Append(" BETWEEN ").Append(start).Append(" AND ").AppendSpace(end);
    public SqlString AndIsNull(string name, string prefix = null) => And().Column(name, prefix).Append(" IS NULL ");
    public SqlString AndIsNotNull(string name, string prefix = null) => And().Column(name, prefix).Append(" IS NOT NULL ");

    public SqlString Or() => Append("\n\rOR ");
    public SqlString Or(string name, string prefix = null) => Or().Opt(name, prefix);

    public SqlString OrderByDesc(object cols, string prefix = null) => OrderBy(cols, "DESC", prefix);
    public SqlString OrderBy(object cols, string dir = "ASC", string prefix = null) =>
        Append("ORDER BY ").ForEachCol(cols, col => Column(col, prefix).AppendSpace().Append(dir).Append(", ")).RemoveTrail(2);

    public SqlString ThenByDesc(object cols, string prefix = null) => ThenBy(cols, "DESC", prefix);
    public SqlString ThenBy(object cols, string dir = "ASC", string prefix = null) =>
        ForEachCol(cols, col => Append(", ").Column(col, prefix).AppendSpace().Append(dir).Append(", ")).RemoveTrail(2);

    public SqlString Limit(int pageNum, int pageSize)
    {
        return Dialect switch
        {
            DbDialect.MySQL => Append('\n').Append($"LIMIT {pageSize * (pageNum - 1)},{pageSize}").End(),
            DbDialect.SQLite => throw new NotImplementedException(),
            DbDialect.SQLServer => throw new NotImplementedException(),
            DbDialect.PostgreSQL => throw new NotImplementedException(),
            _ => throw new NotSupportedException("To call the InsertedId method, you need to set the DbType first.")
        };
    }

    public SqlString Column(string name, string prefix = null)
    {
        if (!string.IsNullOrEmpty(prefix))
        {
            Append(prefix).Append('.');
        }
        return Append(name);
    }

    public SqlString Opt(string name, string opt = "=", string prefix = null) => Column(name, prefix).AppendSpace(opt).Append('@').Append(name);

    public SqlString RemoveTrail(int charCount = 1)
    {
        _sb.Remove(_sb.Length - charCount, charCount);
        return this;
    }

    public SqlString ForEachCol(object obj, Action<string> action, Func<string, bool> predicate = null)
    {
        Guard.NotNull(obj, nameof(obj));
        foreach (var name in Sql.ParamNames(obj, predicate))
        {
            action(name);
        }
        return this;
    }
}
