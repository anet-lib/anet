using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public const string DefaultFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public DateTimeConverter(string format)
        {
            Format = format ?? DefaultFormat;
        }

        public string Format { get; }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}

