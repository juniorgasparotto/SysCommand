using Newtonsoft.Json;
using System;
using System.Reflection;

namespace SysCommand.TestUtils
{
    public class TestObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                   objectType.IsSubclassOf(typeof(PropertyInfo))
                || objectType.IsSubclassOf(typeof(ParameterInfo))
                || objectType.IsSubclassOf(typeof(MethodInfo));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }
}
