using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SysCommand.Compatibility;
using System.Runtime.CompilerServices;

namespace SysCommand.Helpers
{
    public static class ReflectionHelper
    {
        public static bool MethodsAreEquals(MethodInfo first, MethodInfo second)
        {
            first = first.DeclaringType.GetMethod(first.Name, first.GetParameters().Select(p => p.ParameterType).ToArray());
            second = second.DeclaringType.GetMethod(second.Name, second.GetParameters().Select(p => p.ParameterType).ToArray());
            return first == second;
        }

        public static object InvokeWithNamedParameters(this MethodBase self, object obj, IDictionary<string, object> namedParameters)
        {
            //try
            //{
                if (namedParameters == null)
                    return self.Invoke(obj, null);
                return self.Invoke(obj, MapParameters(self, namedParameters));
            //}
            //catch(TargetInvocationException ex)
            //{
            //    throw ex.InnerException;
            //}
        }

        public static object[] MapParameters(MethodBase method, IDictionary<string, object> namedParameters)
        {
            string[] paramNames = method.GetParameters().Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Type.Missing;
            }
            foreach (var item in namedParameters)
            {
                var paramName = item.Key;
                var paramIndex = Array.IndexOf(paramNames, paramName);
                parameters[paramIndex] = item.Value;
            }
            return parameters;
        }

        public static string CSharpName(Type type, bool showFullName = false)
        {
            var sb = new StringBuilder();
            var name = showFullName ? type.FullName : type.Name;
            //return name;
            if (!type.IsGenericType() || name.IndexOf('`') == -1) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(t => CSharpName(t, showFullName))));
            sb.Append(">");
            return sb.ToString();
        }
        
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyLambda)
        {
            MemberExpression Exp = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (propertyLambda.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)propertyLambda.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (propertyLambda.Body is MemberExpression)
            {
                Exp = (MemberExpression)propertyLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)Exp.Member;
        }

        public static bool IsEnum(Type type)
        {
            return GetTypeOrTypeOfNullable(type).IsEnum();
        }

        public static Type GetTypeOrTypeOfNullable(Type type)
        {
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0];
            return type;
        }

        public static object GetDefaultForType(Type targetType)
        {
            return targetType.IsValueType() ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// Get a empty array of Type
        /// </summary>
        /// <returns>Empty array of Type</returns>
        public static Type[] T() { return new Type[] { }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1>() { return new Type[] { typeof(T1) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2>() { return new Type[] { typeof(T1), typeof(T2) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5, T6>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5, T6, T7>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5, T6, T7, T8>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5, T6, T7, T8, T9>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }; }

        /// <summary>
        /// Get a array of type(T)
        /// </summary>
        /// <returns>Array of type(T)</returns>
        public static Type[] T<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }; }

        public static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
