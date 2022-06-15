using System.Text.Json;

namespace Anet.Utilities;

public class Json
{
    private static readonly JsonSerializerOptions _camelCaseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly JsonSerializerOptions _snakeCaseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
    };

    public static string Serialize(object value, JsonSerializerOptions options = null)
    {
        return JsonSerializer.Serialize(value, options);
    }

    public static T Deserialize<T>(string json, JsonSerializerOptions options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }

    public static string SerializeSnakeCase(object value)
    {
        return Serialize(value, _snakeCaseOptions);
    }

    public static T DeserializeSnakeCase<T>(string json)
    {
        return Deserialize<T>(json, _snakeCaseOptions);
    }

    public static string SerializeCamelCase(object value)
    {
        return Serialize(value, _camelCaseOptions);
    }

    public static T DeserializeCamelCase<T>(string json)
    {
        return Deserialize<T>(json, _camelCaseOptions);
    }


    public sealed class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToSnakeCase();
    }
}

