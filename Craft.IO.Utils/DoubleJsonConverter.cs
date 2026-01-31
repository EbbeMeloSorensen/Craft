using System.Globalization;
using Newtonsoft.Json;

namespace Craft.IO.Utils
{
    // I use this when serializing json to avoid writing trailing zeros for doubles,
    // For example: We have a variable of type double, and we assign it the value 4
    //              When we serialize the variable using a standard json serializer then
    //              the value is written as 4.0 - but we want it to be written as 4
    public class DoubleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((double)value).ToString("G", CultureInfo.InvariantCulture));
        }
    }
}
