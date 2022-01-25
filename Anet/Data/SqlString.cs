using System.Reflection;
using System.Text;

namespace Anet.Data;

public class SqlString
{
    readonly StringBuilder _sb;
    private int _selectLength = -1;
    private int _orderStart = -1;

    public SqlString(DbDialect dialect) : this(dialect, null)
    {
    }

    public SqlString(DbDialect dialect, string value)
    {
        _sb = new StringBuilder(value);
        Dialect = dialect;
    }

    public static implicit operator string(SqlString s) => s.ToString();
    public override string ToString() => _sb.ToString();

    public int Length { get => _sb.Length; }

    public string RawString { get => _sb.ToString(); }

    public DbDialect Dialect { get; init; }

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
        _selectLength = -1;
        _orderStart = -1;
        return this;
    }

    public SqlString Line(string value = null)
    {
        if (Length > 0) Append(Environment.NewLine);
        return Append(value);
    }

    public SqlString LineTab(string value = null) =>
        Line("  ").Append(value);

    public SqlString Select(object cols, string prefix = null)
    {
        if (Length > 0) // 已有部分SQL，在其前面插入
        {
            var newSelect = new SqlString(Dialect).Select(cols, prefix);
            if (_selectLength > 0) // 已有SELECT，先移除
            {
                Remove(0, _selectLength);
                _selectLength = newSelect.Length;
            }
            else
            {
                _selectLength = newSelect.Length;
                newSelect.Line();
            }
            Prepend(newSelect);
        }
        else
        {
            Append("SELECT ")
                .ForEachCol(cols, n => Column(n, prefix).Append(", "))
                .RemoveTrail(2);
            _selectLength = Length;
        }
        return this;
    }

    public SqlString Select(string table, object clause) =>
        Select("*").From(table).Where(clause);

    public string Count(string col = "1") =>
        DeOrder().Select($"COUNT({col})");

    public SqlString From(string table) =>
         Line("FROM ").Append(table);

    public SqlString Update(string table, object update, object clause) =>
        Line("UPDATE ").Append(table).
        Line("SET ").ForEachCol(update, name => Opt(name).Append(", ")).RemoveTrail(2).
        Where(clause);

    public SqlString Insert(string table, object obj) =>
        Line("INSERT INTO ").Append(table).Append('(').ForEachCol(obj, n => Append(n).Append(", ")).RemoveTrail(2).Append(')').
        Line("VALUES (").ForEachCol(obj, n => Append('@').Append(n).Append(", ")).RemoveTrail(2).Append(')');

    public SqlString InsertedId()
    {
        return Dialect switch
        {
            DbDialect.MySQL => Select(";LAST_INSERT_ID()"),
            DbDialect.SQLite => Select(";LAST_INSERT_ROWID()"),
            DbDialect.SQLServer => Select(";SCOPE_IDENTITY()"),
            DbDialect.PostgreSQL => Select(";LASTVAL()"),
            _ => throw new NotSupportedException("Not supported DbDialect."),
        };
    }

    public SqlString Delete(string table) =>
        Line("DELETE FROM ").Append(table);

    public SqlString Delete(string table, object clause) =>
        Delete(table).Where(clause);

    public SqlString Where() => Line("WHERE 1=1 ");

    public SqlString Where(object obj, string prefix = null) =>
        Where().And(obj, prefix);

    public SqlString And(object obj, string prefix = null) =>
        ForEachCol(obj, name => Append("AND ").Opt(name, "=", prefix));

    public SqlString Order(string by)
    {
        _orderStart = _sb.Length;
        return Line("ORDER BY ").Append(by);
    }

    public SqlString DeOrder()
    {
        if (_orderStart != -1)
        {
            Remove(_orderStart, _sb.Length - _orderStart);
            _orderStart = -1;
        }
        return this;
    }

    public SqlString Page(int pageNo, int pageSize)
    {
        if (_orderStart < 0)
        {
            throw new InvalidOperationException("Missing the Order method before the Page method.");
        }
        var offset = pageSize * (pageNo - 1);
        return Dialect switch
        {
            DbDialect.MySQL => Line($"LIMIT {offset},{pageSize}"),
            DbDialect.SQLite => Line($"LIMIT {pageSize} OFFSET {offset}"),
            DbDialect.SQLServer => Line($"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY"),
            DbDialect.PostgreSQL => Line($"LIMIT {pageSize} OFFSET {offset}"),
            _ => throw new NotSupportedException("Not supported DbDialect."),
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

    public SqlString Opt(string name, string opt = "=", string prefix = null) =>
        Column(name, prefix).Append(opt).Append('@').Append(name);

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

        IEnumerable<string> names;

        if (obj is string str)
        {
            names = str.Split(',');
        }
        else if (obj is IEnumerable<string> list)
        {
            names = list;
        }
        else if (obj is Type type)
        {
            names = GetPropsFields(type);
        }
        else
        {
            names = GetPropsFields(obj.GetType().GetAnyElementType());
        }

        if (predicate != null)
        {
            names = names.Where(predicate);
        }

        return names;
    }

    private static IEnumerable<string> GetPropsFields(Type type) => type
        .GetFields(_colBind).Where(x => x.FieldType.IsSimpleType()).Select(x => x.Name)
        .Concat(type.GetProperties(_colBind).Where(x => x.PropertyType.IsSimpleType()).Select(x => x.Name));

    #endregion
}
