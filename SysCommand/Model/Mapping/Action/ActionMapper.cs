//using System.Collections.Generic;
//using System.Linq;
//using System;
//using System.Reflection;

//namespace SysCommand.Mapping
//{
//    public class ActionMapper
//    {
//        public const string MAIN_METHOD_NAME = "main";
//        private ArgumentMapper mapperArgument = new ArgumentMapper();

//        public IEnumerable<ActionMap> Map(object target, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
//        {
//            var maps = new List<ActionMap>();
//            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic && !f.IsSpecialName).ToArray();
//            return Map(target, methods, onlyWithAttribute, usePrefixInAllMethods, prefix);
//        }

//        public IEnumerable<ActionMap> Map(object target, MethodInfo[] methods, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
//        {
//            var maps = new List<ActionMap>();

//            foreach (var method in methods)
//            {
//                var attribute = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;

//                var isMainMethod = method.Name.ToLower() == MAIN_METHOD_NAME;
//                var countParameters = method.GetParameters().Length;

//                // the main method, with zero arguments, can't be a action
//                if (isMainMethod && countParameters == 0 && attribute == null)
//                    continue;
//                else if (isMainMethod && countParameters == 0 && attribute != null)
//                    throw new Exception("The main method, with zero arguments, can be se a action. This method is reserved to be aways invoked in action.   it is defined.");

//                var ignoredByWithoutAttr = onlyWithAttribute && attribute == null && !isMainMethod;
//                var ignoredByMethod = attribute != null && attribute.Ignore;

//                if (ignoredByWithoutAttr || ignoredByMethod)
//                    continue;

//                string actionNameRaw;
//                string actionName;

//                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
//                    actionNameRaw = attribute.Name;
//                else
//                    actionNameRaw = mapperArgument.GetLongName(method.Name);

//                var usePrefix = attribute == null ? true : attribute.UsePrefix;
//                var usePrefixFinal = usePrefixInAllMethods && usePrefix;
//                if (usePrefixFinal)
//                {
//                    if (string.IsNullOrWhiteSpace(prefix))
//                        prefix = mapperArgument.GetLongNameByType(target.GetType());

//                    actionName = prefix + "-" + actionNameRaw;
//                }
//                else
//                {
//                    actionName = actionNameRaw;
//                }

//                var isDefaultAction = false;
//                if (isMainMethod || (attribute != null && attribute.IsDefault))
//                    isDefaultAction = true;

//                var enablePositionalArgs = attribute == null ? true : attribute.EnablePositionalArgs;
//                var argsMaps = GetArgumentsMapsFromMethod(target, method);
//                maps.Add(new ActionMap(target, method, actionName, prefix, actionNameRaw, usePrefixFinal, isDefaultAction, enablePositionalArgs, argsMaps));
//            }

//            return maps;
//        }

//        public IEnumerable<ArgumentMap> GetArgumentsMapsFromMethod(object target, MethodInfo method)
//        {
//            var maps = new List<ArgumentMap>();
//            var parameters = method.GetParameters();
//            foreach (var parameter in parameters)
//            {
//                var map = GetArgumentMap(target, parameter);
//                maps.Add(map);
//            }

//            mapperArgument.Validate(maps, method.Name);

//            return mapperArgument.SortByPosition(maps);
//        }

//        public ArgumentMap GetArgumentMap(object target, ParameterInfo parameter)
//        {
//            var attribute = Attribute.GetCustomAttribute(parameter, typeof(ArgumentAttribute)) as ArgumentAttribute;

//            string longName = null;
//            char? shortName = null;
//            string helpText = null;
//            bool showHelpComplement = false;
//            int? position = null;
//            bool hasPosition = false;
//            bool isOptional = parameter.IsOptional;
//            bool hasDefaultValue = parameter.HasDefaultValue;
//            object defaultValue = parameter.DefaultValue;

//            if (attribute != null)
//            {
//                longName = attribute.LongName;
//                shortName = attribute.ShortName;
//                helpText = attribute.Help;
//                showHelpComplement = attribute.ShowHelpComplement;
//                hasPosition = attribute.HasPosition;
//                position = (int?)attribute.Position;
//            }

//            return mapperArgument.Map(target, parameter, parameter.Name, parameter.ParameterType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
//        }

//    }
//}
