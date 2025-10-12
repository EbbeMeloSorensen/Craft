using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Craft.IO.Utils
{
    public class NullableDoubleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteRawValue("");
            }
            else
            {
                writer.WriteRawValue((((double?)value).Value).ToString("G", CultureInfo.InvariantCulture));
            }
        }
    }
}