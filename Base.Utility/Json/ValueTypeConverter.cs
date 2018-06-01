using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility
{
    public class ValueTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsValueType && (objectType == typeof(int) || objectType == typeof(uint) || objectType == typeof(int?) || objectType == typeof(uint?) || objectType == typeof(short) || objectType == typeof(ushort) || objectType == typeof(short?) || objectType == typeof(ushort?) || objectType == typeof(long) || objectType == typeof(ulong) || objectType == typeof(long?) || objectType == typeof(ulong?) || objectType == typeof(decimal) || objectType == typeof(decimal?) || objectType == typeof(double) || objectType == typeof(double?) || objectType == typeof(float) || objectType == typeof(float?) || objectType == typeof(DateTime) || objectType == typeof(DateTime?) || objectType == typeof(bool) || objectType == typeof(bool?) || objectType == typeof(char) || objectType == typeof(char?) || objectType == typeof(byte) || objectType == typeof(byte?) || objectType == typeof(sbyte) || objectType == typeof(sbyte?));
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type type;
            if (objectType.IsGenericType)
            {
                type = objectType.GetGenericArguments()[0];
            }
            else
            {
                type = objectType;
            }
            if (reader.Value == null)
            {
                if (!objectType.IsGenericType)
                {
                    throw new Exception(string.Format("{0}需要为Nullable才能接受null！", reader.Path));
                }
                return null;
            }
            else
            {
                if (reader.Value as string == "")
                {
                    if (objectType.IsGenericType)
                    {
                        return null;
                    }
                    return Activator.CreateInstance(type);
                }
                else
                {
                    if (reader.ValueType != type)
                    {
                        return Convert.ChangeType(reader.Value, type);
                    }
                    return reader.Value;
                }
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
