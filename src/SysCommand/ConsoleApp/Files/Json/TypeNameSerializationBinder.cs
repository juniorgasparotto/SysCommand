#if NETCORE

using Newtonsoft.Json.Serialization;
using SysCommand.Compatibility;
using System;

namespace SysCommand.ConsoleApp.Files
{
    internal class TypeNameSerializationBinder : ISerializationBinder
    {
        public TypeNameSerializationBinder()
        {
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly().FullName;
            typeName = serializedType.FullName;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName + ", " + assemblyName, true);
        }
    }
}
#else

using System;
using System.Runtime.Serialization;

namespace SysCommand.ConsoleApp.Files
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

#endif
