using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public static class JsonConvertExtensions
    {
        public static string SerializeForApiEnZona<T>(T data)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var serializer = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };
            serializer.Converters.Add(new DoubleJsonConverter());
            //serializer.Converters.Add(new DateTimeJsonConverter());

            return JsonConvert.SerializeObject(data, serializer);
        }

        public static T DeserializeForApiEnZona<T>(string dataAsJson, Formatting formatting = Formatting.None)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var serializer = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = formatting
            };
            serializer.Converters.Add(new DoubleJsonConverter());
            //serializer.Converters.Add(new DateTimeJsonConverter());

            return JsonConvert.DeserializeObject<T>(dataAsJson, serializer);
        }
    }
}
