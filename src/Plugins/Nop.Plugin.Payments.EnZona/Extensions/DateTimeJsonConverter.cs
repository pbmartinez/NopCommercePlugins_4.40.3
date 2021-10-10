using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return DateTime.MinValue;

            var salida = DateTime.MinValue;
            if (reader.Value is DateTime)
                salida = (DateTime)reader.Value;
            else if (reader.Value is string)
            {
                salida = ((string)reader.Value).AsDateTime();
            }
            return salida;
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            var salida = $"{value:yyyy-MM-ddTHH:mm:ss.fffzzz}";

            writer.WriteValue(salida);
        }
    }
}
