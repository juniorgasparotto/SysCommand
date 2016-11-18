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
