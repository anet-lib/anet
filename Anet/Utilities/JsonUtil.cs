using System.Text.Encodings.Web;
using System.Text.Json;

namespace Anet.Utilities;

public class JsonUtil
{
    private static readonly JsonSerializerOptions _defaultOptions= new()
    {
        // ref: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/character-encoding
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

        PropertyNameCaseInsensitive = true,
    };

    private static readonly JsonSerializerOptions _camelCaseOptions = new()
    {
        Encoder = _defaultOptions.Encoder,
        PropertyNameCaseInsensitive = _defaultOptions.PropertyNameCaseInsensitive,

        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly JsonSerializerOptions _snakeCaseOptions = new()
    {
        Encoder = _defaultOptions.Encoder,
        PropertyNameCaseInsensitive = _defaultOptions.PropertyNameCaseInsensitive,

        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    internal static void SetDefaultSerializerOptions(Action<JsonSerializerOptions> configure)
    {
        configure?.Invoke(_defaultOptions);
    }

    public static string Serialize(object value, JsonSerializerOptions options = null)
    {
        return JsonSerializer.Serialize(value, options ?? _defaultOptions);
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
}

