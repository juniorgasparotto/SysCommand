using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SysCommand.Mapping;
using SysCommand.Compatibility;

namespace SysCommand.DefaultExecutor
{
    /// <summary>
    /// Represent a mapper of list of ActionMap
    /// </summary>
    public class ActionMapper
    {
        private ArgumentMapper argumentMapper;

        /// <summary>
        /// Initialize the mapper
        /// </summary>
        /// <param name="argumentMapper">Mapper of arguments</param>
        public ActionMapper(ArgumentMapper argumentMapper)
        {
            this.argumentMapper = argumentMapper;
        }
        
        /// <summary>
        /// Create a map of actions from the specific target object
        /// </summary>
        /// <param name="target">Object to be mapped</param>
        /// <param name="onlyWithAttribute">Ignore all methods that do not have the ActionAttribute</param>
        /// <param name="usePrefixInAllMethods">Determine whether the methods are prefixed</param>
        /// <param name="prefix">Prefix text is enabled</param>
        /// <returns>List of ActionMap</returns>
        public IEnumerable<ActionMap> Map(object target, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();
            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic && !f.IsSpecialName).ToArray();
            return Map(target, methods, onlyWithAttribute, usePrefixInAllMethods, prefix);
        }

        /// <summary>
        /// Create a map of actions from the specific target object
        /// </summary>
        /// <param name="target">Object to be mapped</param>
        /// <param name="methods">Methods to be mapped</param>
        /// <param name="onlyWithAttribute">Ignore all methods that do not have the ActionAttribute</param>
        /// <param name="usePrefixInAllMethods">Determine whether the methods are prefixed</param>
        /// <param name="prefix">Prefix text is enabled</param>
        /// <returns>List of ActionMap</returns>
        public IEnumerable<ActionMap> Map(object target, MethodInfo[] methods, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();

            foreach (var method in methods)
            {
                var attribute = ReflectionCompatibility.GetCustomAttribute<ActionAttribute>(method);

                var isMainMethod = method.Name.ToLower() == Executor.MAIN_METHOD_NAME;
                var countParameters = method.GetParameters().Length;

                // the main method, with zero arguments, can't be a action
                if (isMainMethod && countParameters == 0 && attribute == null)
                    continue;
                else if (isMainMethod && countParameters == 0 && attribute != null)
                    throw new Exception("The main method, with zero arguments, can be se a action. This method is reserved to be aways invoked in action.   it is defined.");

                var ignoredByWithoutAttr = onlyWithAttribute && attribute == null && !isMainMethod;
                var ignoredByMethod = attribute != null && attribute.Ignore;

                if (ignoredByWithoutAttr || ignoredByMethod)
                    continue;

                string actionNameRaw;
                string actionName;

                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
                    actionNameRaw = attribute.Name;
                else
                    actionNameRaw = argumentMapper.GetLongName(method.Name);

                var usePrefix = attribute == null ? true : attribute.UsePrefix;
                var usePrefixFinal = usePrefixInAllMethods && usePrefix;
                if (usePrefixFinal)
                {
                    if (string.IsNullOrWhiteSpace(prefix))
                        prefix = argumentMapper.GetPrefixByType(target.GetType());

                    actionName = prefix + "-" + actionNameRaw;
                }
                else
                {
                    actionName = actionNameRaw;
                }

                var isDefaultAction = false;
                if (isMainMethod || (attribute != null && attribute.IsDefault))
                    isDefaultAction = true;

                var helpText = attribute == null ? null : attribute.Help;
                var enablePositionalArgs = attribute == null ? true : attribute.EnablePositionalArgs;

                var argsMaps = this.argumentMapper.GetFromMethod(target, method);
                maps.Add(new ActionMap(target, method, actionName, prefix, actionNameRaw, usePrefixFinal, helpText, isDefaultAction, enablePositionalArgs, argsMaps));
            }

            return maps;
        }

    }
}
