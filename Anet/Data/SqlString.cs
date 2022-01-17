using System.Reflection;
using System.Text;

namespace Anet.Data;

public class SqlString
{
    readonly StringBuilder _sb;

    public SqlString(DbDialect dialect) : this(dialect, null)
    {
    }
    public SqlString(DbDialect dialect, string value)
    {
        _sb = new StringBuilder(value);
        Dialect = dialect;
    }

    public int Length { get => _sb.Length; }

    public string RawString { get => _sb.ToString(); }

    public DbDialect Dialect { get; init; }

    public static implicit operator string(SqlString s) => s.ToString();
    //public static implicit operator SqlString(string s) => new(s);

    public override string ToString() => _sb.ToString();
    public SqlString Clone() => new(Dialect, this);

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
    public SqlString Remove(int start, int length)
    {
        _sb.Remove(start, length);
        return this;
    }
    public SqlString Clear()
    {
        _sb.Clear();
        return this;
    }

    public SqlString AppendLine(string value = null) => Append(value).Append('\n');
    public SqlString AppendSpace(string value = null) => Append(value).Append(' ');
    public SqlString LineAppend(string value = null) => Append('\n').Append(value);
    public SqlString TabAppend(string value = null) => Append("\n  ").Append(value);
    public SqlString End() => Append(';');

    private int _selectLength = -1;
    public SqlString Select(object cols, string prefix = null)
    {
        if (Length > 0) // 已有部分SQL，在其前面插入
        {
            if (_selectLength > 0) // 已有SELECT，先移除
                Remove(0, _selectLength);
            var newSelect = new SqlString(Dialect).Select(cols, prefix);
            Prepend(newSelect);
            _selectLength = newSelect.Length;
        }
        else
        {
            Append("SELECT ").ForEachCol(cols, n => Column(n, prefix).Append(", ")).RemoveTrail(2);
            _selectLength = Length;
        }
        return this;
    }
    public SqlString DeSelect()
    {
        if (_selectLength > 0)
        {
            Remove(0, _selectLength);
            _selectLength = -1;
        }
        return this;
    }

    public SqlString Count(string col = "1") => Select($"COUNT({col})");

    public SqlString From(string table, string alias = null)
    {
        LineAppend("FROM ").Append(table);
        if (!string.IsNullOrEmpty(alias))
            Append(" AS ").Append(alias);
        return AppendSpace();
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
            DbDialect.SQLite => End().Select("LAST_INSERT_ROWID()"),
            DbDialect.SQLServer => End().Select("SCOPE_IDENTITY()"),
            DbDialect.PostgreSQL => End().Select("LASTVAL()"),
            _ => throw new NotSupportedException("To call the InsertedId method, you need to set the DbDialect first."),
        };
    }

    public SqlString Delete(string table) => Append("DELETE FROME ").AppendSpace(table);

    public SqlString Where() => LineAppend("WHERE 1=1 ");
    public SqlString Where(object obj, string prefix = null) => Where().And(obj, prefix);

    public SqlString And() => TabAppend("AND ");
    public SqlString And(object obj, string prefix = null) => ForEachCol(obj, name => And().Opt(name, "=", prefix));
    public SqlString AndIn(string name, string prefix = null) => And().Opt(name, " IN ", prefix);
    public SqlString AndLike(string name, string prefix = null) => And().Opt(name, " LIKE ", prefix);
    public SqlString AndBetween(string name, string start, string end, string prefix = null) =>
        And().Column(name, prefix).Append(" BETWEEN ").Append(start).Append(" AND ").AppendSpace(end);
    public SqlString AndIsNull(string name, string prefix = null) => And().Column(name, prefix).Append(" IS NULL");
    public SqlString AndIsNotNull(string name, string prefix = null) => And().Column(name, prefix).Append(" IS NOT NULL");

    public SqlString Or() => TabAppend("OR ");
    public SqlString Or(string name, string prefix = null) => Or().Opt(name, prefix);

    public SqlString OrderByDesc(object cols, string prefix = null) => OrderBy(cols, "DESC", prefix);
    public SqlString OrderBy(object cols, string dir = "ASC", string prefix = null) =>
        LineAppend("ORDER BY ").ForEachCol(cols, col => Column(col, prefix).AppendSpace().Append(dir).Append(", ")).RemoveTrail(2);

    public SqlString ThenByDesc(object cols, string prefix = null) => ThenBy(cols, "DESC", prefix);
    public SqlString ThenBy(object cols, string dir = "ASC", string prefix = null) =>
        AppendSpace().ForEachCol(cols, col => Append(", ").Column(col, prefix).AppendSpace().Append(dir).Append(", ")).RemoveTrail(2);

    private int _pageStart = -1;
    public SqlString Page(int pageNum, int pageSize)
    {
        _pageStart = _sb.Length;
        return Dialect switch
        {
            DbDialect.MySQL => LineAppend($"LIMIT {pageSize * (pageNum - 1)},{pageSize}"),
            DbDialect.SQLite => LineAppend($"LIMIT {pageSize} OFFSET {pageSize * (pageNum - 1)}"),
            DbDialect.SQLServer => LineAppend($"OFFSET {pageSize * (pageNum - 1)} ROWS FETCH NEXT {pageSize} ROWS ONLY"),
            DbDialect.PostgreSQL => LineAppend($"LIMIT {pageSize} OFFSET {pageSize * (pageNum - 1)}"),
            _ => throw new NotSupportedException("To call the InsertedId method, you need to set the DbDialect first.")
        };
    }
    public SqlString DePage()
    {
        Remove(0, _pageStart);
        _pageStart = -1;
        return this;
    }

    public SqlString Column(string name, string prefix = null)
    {
        if (!string.IsNullOrEmpty(prefix))
        {
            Append(prefix).Append('.');
        }
        return Append(name);
    }

    public SqlString Opt(string name, string opt = "=", string prefix = null) => Column(name, prefix).Append(opt).Append('@').Append(name);

    public SqlString RemoveTrail(int charCount = 1)
    {
        _sb.Remove(_sb.Length - charCount, charCount);
        return this;
    }

    public SqlString ForEachCol(object obj, Action<string> action, Func<string, bool> predicate = null)
    {
        ArgumentNullException.ThrowIfNull(obj);
        foreach (var name in ParamNames(obj, predicate))
        {
            action(name);
        }
        return this;
    }

    public SqlString Select(string table, object clause)
    {
        return Select("*").From(table).Where(clause);
    }

    public SqlString Insert(string table, object columns)
    {
        return Insert(table).Values(columns);
    }

    public SqlString Update(string table, object update, object clause)
    {
        return Update(table).Values(update).Where(clause);
    }

    public SqlString Delete(string table, object clause)
    {
        return Delete(table).Where(clause);
    }

    #region Static Methods

    static readonly BindingFlags _colBind = BindingFlags.Instance | BindingFlags.Public;

    public static IEnumerable<string> ParamNames(object obj, string excludeCols)
    {
        var excludes = excludeCols.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return ParamNames(obj, n => !excludes.Contains(n));
    }

    public static IEnumerable<string> ParamNames(object obj, Func<string, bool> predicate = null)
    {
        ArgumentNullException.ThrowIfNull(obj);

        static IEnumerable<string> GetPropsFields(Type type) => type
            .GetFields(_colBind).Select(x => x.Name)
            .Concat(type.GetProperties(_colBind).Select(x => x.Name));

        IEnumerable<string> names;

        if (obj is string str)
        {
            names = str.Split(',');
        }
        else if (obj is IEnumerable<string> list)
        {
            names = list.Where(predicate);
        }
        else if (obj is Type type)
        {
            names = GetPropsFields(type);
        }
        else
        {
            // todo: 限制类型
            // throw new NotSupportedException();
            names = GetPropsFields(obj.GetType());
        }

        if (predicate != null)
        {
            names = names.Where(predicate);
        }

        return names;
    }

    #endregion
}
