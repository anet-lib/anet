using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Anet
{
    public class Json
    {
        public static JsonSerializerSettings SnakeCaseSettings
        {
            get => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public static JsonSerializerSettings CamelCaseSettings 
        {
            get => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        public static string Serialize(object value, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T Deserialize<T>(string json, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string SerializeSnakeCase(object value)
        {
            return Serialize(value, SnakeCaseSettings);
        }

        public static T DeserializeSnakeCase<T>(string json)
        {
            return Deserialize<T>(json, SnakeCaseSettings);
        }

        public static string SerializeCamelCase(object value)
        {
            return Serialize(value, CamelCaseSettings);
        }

        public static T DeserializeCamelCase<T>(string json)
        {
            return Deserialize<T>(json, CamelCaseSettings);
        }
    }
}
