using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Test
{
    public class TestObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                   objectType.IsSubclassOf(typeof(System.Reflection.PropertyInfo))
                || objectType.IsSubclassOf(typeof(System.Reflection.ParameterInfo))
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
