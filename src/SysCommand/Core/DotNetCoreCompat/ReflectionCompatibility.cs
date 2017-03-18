using System;
using System.Reflection;

#if NETCORE
using System.Collections.Generic;
using System.Linq;
#endif

namespace SysCommand.Compatibility
{
    public static class ReflectionCompatibility
    {
#if NETCORE
        public static bool IsDefined(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == attributeType);
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttribute<T>(inherit);
        }

        public static bool IsSubclassOf(this Type type, Type typeCheck)
        {
            return type.GetTypeInfo().IsSubclassOf(typeCheck);
        }
#endif

        public static T GetCustomAttribute<T>(MethodInfo method) where T : Attribute
        {
            Type typeAttr = typeof(T);

#if NETCORE
            var attr = method.GetCustomAttributes(typeAttr).FirstOrDefault();
            if (attr != null)
                return attr as T;
            return null;
#else
            return Attribute.GetCustomAttribute(method, typeAttr) as T;
#endif
        }

        public static T GetCustomAttribute<T>(PropertyInfo property) where T : Attribute
        {
            Type typeAttr = typeof(T);

#if NETCORE
            var attr = property.GetCustomAttributes(typeAttr).FirstOrDefault();
            if (attr != null)
                return attr as T;
            return null;
#else
            return Attribute.GetCustomAttribute(property, typeAttr) as T;
#endif
        }

        public static T GetCustomAttribute<T>(ParameterInfo parameter) where T : Attribute
        {
            Type typeAttr = typeof(T);

#if NETCORE
            var attr = parameter.GetCustomAttributes(typeAttr).FirstOrDefault();
            if (attr != null)
                return attr as T;
            return null;
#else
            return Attribute.GetCustomAttribute(parameter, typeAttr) as T;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static MethodInfo Method(this Delegate d)
        {
#if NETCORE
            return d.GetMethodInfo();
#else
            return d.Method;
#endif
        }

        public static Assembly Assembly(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static Assembly[] GetAssemblies()
        {
#if NETCORE
            var curAssembly = System.Reflection.Assembly.GetEntryAssembly();
            List<Assembly> assemblies = new List<Assembly>
            {
                curAssembly
            };
            return assemblies.ToArray();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if NETCORE
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }
    }
}
