using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server_TSD.Models.HelperClass
{
    public class Utils
    {
        // Кастомный конвертер для даты
        public class DateTimeJsonConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
        }
    }
}
