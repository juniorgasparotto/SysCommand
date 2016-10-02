using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace SysCommand
{
    internal class TypeNameSerializationBinder : SerializationBinder
    {
        public TypeNameSerializationBinder()
        {
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName + ", " + assemblyName, true);
        }
    }
}
