using System.Reflection;

namespace Anet.Data;

public static class Sql
{
    public static SqlString Select(string table, object clause)
    {
        return new SqlString().Select("*").From(table).Where(clause);
    }

    public static SqlString Insert(string table, object columns)
    {
        return new SqlString().Insert(table).Values(columns);
    }

    public static SqlString Update(string table, object update, object clause)
    {
       return new SqlString().Update(table).Values(update).Where(clause);
    }

    public static SqlString Delete(string table, object clause)
    {
        return new SqlString().Delete(table).Where(clause);
    }

    static readonly BindingFlags _colBind = BindingFlags.Instance & BindingFlags.Public;

    public static IEnumerable<string> ParamNames(object obj, string excludeCols)
    {
        var excludes = excludeCols.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return ParamNames(obj, n => !excludes.Contains(n));
    }

    public static IEnumerable<string> ParamNames(object obj, Func<string, bool> predicate = null)
    {
        Guard.NotNull(obj, nameof(obj));

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
            names = type
                .GetFields(_colBind).Select(x => x.Name)
                .Concat(type.GetProperties(_colBind).Select(x => x.Name))
                .Where(predicate);
        }
        else
        {
            throw new NotSupportedException();
        }

        if (predicate != null)
        {
            names = names.Where(predicate) ;
        }

        return names;
    }
}
