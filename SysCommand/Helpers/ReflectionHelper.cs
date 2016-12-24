using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SysCommand.Helpers
{
    public static class ReflectionHelper
    {
        public static bool MethodsAreEquals(MethodInfo first, MethodInfo second)
        {
            first = first.ReflectedType == first.DeclaringType ? first : first.DeclaringType.GetMethod(first.Name, first.GetParameters().Select(p => p.ParameterType).ToArray());
            second = second.ReflectedType == second.DeclaringType ? second : second.DeclaringType.GetMethod(second.Name, second.GetParameters().Select(p => p.ParameterType).ToArray());
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
            if (!type.IsGenericType || name.IndexOf('`') == -1) return name;
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

        public static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            Type t = typeof(T);

            ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null);

            return (T)ci.Invoke(paramValues);
        }

        public static bool IsEnum(Type type)
        {
            return GetTypeOrTypeOfNullable(type).IsEnum;
        }

        public static Type GetTypeOrTypeOfNullable(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0];
            return type;
        }

        public static object GetDefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
