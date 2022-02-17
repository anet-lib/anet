using System.Text;

namespace System;

public static class StringExtensions
{
    public static bool Equals(this string source, string value, bool ignoreCase)
    {
        return source.Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    public static string ToSnakeCase(this string source)
    {
        if (string.IsNullOrEmpty(source)) return source;

        //return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();

        var sb = new StringBuilder();
        var state = SnakeCaseState.Start;

        var nameSpan = source.AsSpan();

        for (int i = 0; i < nameSpan.Length; i++)
        {
            if (nameSpan[i] == ' ')
            {
                if (state != SnakeCaseState.Start)
                {
                    state = SnakeCaseState.NewWord;
                }
            }
            else if (char.IsUpper(nameSpan[i]))
            {
                switch (state)
                {
                    case SnakeCaseState.Upper:
                        bool hasNext = i + 1 < nameSpan.Length;
                        if (i > 0 && hasNext)
                        {
                            char nextChar = nameSpan[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != '_')
                            {
                                sb.Append('_');
                            }
                        }
                        break;
                    case SnakeCaseState.Lower:
                    case SnakeCaseState.NewWord:
                        sb.Append('_');
                        break;
                }
                sb.Append(char.ToLowerInvariant(nameSpan[i]));
                state = SnakeCaseState.Upper;
            }
            else if (nameSpan[i] == '_')
            {
                sb.Append('_');
                state = SnakeCaseState.Start;
            }
            else
            {
                if (state == SnakeCaseState.NewWord)
                {
                    sb.Append('_');
                }

                sb.Append(nameSpan[i]);
                state = SnakeCaseState.Lower;
            }
        }

        return sb.ToString();
    }

    private enum SnakeCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }
}
