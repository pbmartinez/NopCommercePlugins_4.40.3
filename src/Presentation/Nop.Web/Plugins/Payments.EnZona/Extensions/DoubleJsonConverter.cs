using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public class DoubleJsonConverter : JsonConverter<double>
    {
        public override double ReadJson(JsonReader reader, Type objectType, double existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var valueAsString = (string)reader.Value;
            _ = double.TryParse(valueAsString, out var salida);
            if (salida > 0)
                salida = Math.Truncate(salida * 100 / 100);
            return salida;
        }

        public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
        {
            var salida = $"{value:N2}";
            writer.WriteValue(salida);
        }
    }
}
