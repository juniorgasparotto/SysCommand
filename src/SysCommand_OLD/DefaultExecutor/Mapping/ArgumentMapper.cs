using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SysCommand.Helpers;
using SysCommand.Mapping;
using SysCommand.Reflection;

namespace SysCommand.DefaultExecutor
{
    public class ArgumentMapper
    {
        public IEnumerable<ArgumentMap> Map(object target, bool onlyWithAttribute = false)
        {
            var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return Map(target, properties, onlyWithAttribute);
        }

        public IEnumerable<ArgumentMap> Map(object target, PropertyInfo[] properties, bool onlyWithAttribute = false)
        {
            var maps = new List<ArgumentMap>();
            foreach (PropertyInfo property in properties)
            {
                var attribute = ReflectionCompatibility.GetCustomAttribute<ArgumentAttribute>(property);

                if (onlyWithAttribute && attribute == null)
                    continue;

                var map = Map(target, property);
                maps.Add(map);
            }

            Validate(maps, target.GetType().Name);

            return SortByPosition(maps);
        }

        public ArgumentMap Map(object target, PropertyInfo property)
        {
            var attribute = ReflectionCompatibility.GetCustomAttribute<ArgumentAttribute>(property);
            string longName = null;
            char? shortName = null;
            string helpText = null;
            bool showHelpComplement = true;
            int? position = null;
            bool hasPosition = false;
            bool isOptional = true;
            bool hasDefaultValue = false;
            object defaultValue = null;

            if (attribute != null)
            {
                longName = attribute.LongName;
                shortName = attribute.ShortName;
                helpText = attribute.Help;
                showHelpComplement = attribute.ShowHelpComplement;
                hasPosition = attribute.HasPosition;
                position = (int?)attribute.Position;
                hasDefaultValue = attribute.HasDefaultValue;
                defaultValue = attribute.DefaultValue;
                isOptional = !attribute.IsRequired;
            }

            return Map(target, property, property.Name, property.PropertyType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public ArgumentMap Map(object target, object targetMember, string mapName, Type mapType, string longName, char? shortName, bool hasPosition, int? position, string helpText, bool showHelpComplement, bool isOptional, bool hasDefaultValue, object defaultValue)
        {
            shortName = shortName == default(char) ? null : shortName;
            position = hasPosition ? position : null;

            if (longName == null && shortName == null)
            {
                if (mapName.Length == 1)
                {
                    shortName = mapName[0].ToString().ToLower()[0];
                }
                else
                {
                    longName = GetLongName(mapName);
                }
            }

            return new ArgumentMap(target, targetMember, mapName, longName, shortName, mapType, isOptional, hasDefaultValue, defaultValue, helpText, showHelpComplement, position);
        }

        public ArgumentMap Map(object target, ParameterInfo parameter)
        {
            var attribute = ReflectionCompatibility.GetCustomAttribute<ArgumentAttribute>(parameter);

            string longName = null;
            char? shortName = null;
            string helpText = null;
            bool showHelpComplement = true;
            int? position = null;
            bool hasPosition = false;
            bool isOptional = parameter.IsOptional;
            bool hasDefaultValue = parameter.HasDefaultValue;
            object defaultValue = parameter.DefaultValue;

            if (attribute != null)
            {
                longName = attribute.LongName;
                shortName = attribute.ShortName;
                helpText = attribute.Help;
                showHelpComplement = attribute.ShowHelpComplement;
                hasPosition = attribute.HasPosition;
                position = (int?)attribute.Position;
            }

            return this.Map(target, parameter, parameter.Name, parameter.ParameterType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public IEnumerable<ArgumentMap> SortByPosition(IEnumerable<ArgumentMap> maps)
        {
            var mapsWithOrder = maps.Where(f => f.Position != null).OrderBy(f => f.Position).ToList();
            if (mapsWithOrder.Count > 0)
            {
                var mapsWithoutOrder = maps.Where(f => f.Position == null);
                var mapsReturn = new List<ArgumentMap>();
                mapsReturn.AddRange(mapsWithOrder);
                mapsReturn.AddRange(mapsWithoutOrder);
                return mapsReturn;
            }
            return maps;
        }

        public string GetLongName(string name)
        {
            return StringHelper.ToLowerSeparate(name, '-');
        }

        public string GetLongNameByType(Type type)
        {
            var prefix = StringHelper.ToLowerSeparate(type.Name, '-');
            var split = prefix.Split('-').ToList();
            if (split.Last() == "command")
            {
                split.RemoveAt(split.Count - 1);
                prefix = string.Join("-", split.ToArray());
            }

            return prefix;
        }
        
        public IEnumerable<ArgumentMap> GetFromMethod(object target, MethodInfo method)
        {
            var maps = new List<ArgumentMap>();
            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                var map = Map(target, parameter);
                maps.Add(map);
            }

            this.Validate(maps, method.Name);

            return this.SortByPosition(maps);
        }


        public void Validate(IEnumerable<ArgumentMap> maps, string contextName)
        {
            foreach (var map in maps)
            {
                if (string.IsNullOrWhiteSpace(map.LongName) && string.IsNullOrWhiteSpace(map.ShortName.ToString()))
                    throw new Exception(string.Format("The map '{0}' there's no values in 'LongName' or 'ShortName' in method or class '{1}'", map.MapName, contextName));

                if (map.ShortName != null && (map.ShortName.Value.ToString().Trim().Length == 0 || !char.IsLetter(map.ShortName.Value)))
                    throw new Exception(string.Format("The map '{0}' has 'ShortName' invalid in method or class '{1}'", map.MapName, contextName));

                if (map.LongName != null && (map.LongName.Trim().Length == 0 || !char.IsLetter(map.LongName[0])))
                    throw new Exception(string.Format("The map '{0}' has 'LongName' invalid in method or class '{1}'", map.MapName, contextName));

                if (map.MapName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.MapName == map.MapName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'MapName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }
                else
                {
                    throw new Exception(string.Format("The method or object '{0}' has a map with no value in the property 'MapName'", contextName));
                }

                if (map.LongName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.LongName == map.LongName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'LogName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }

                if (map.ShortName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.ShortName == map.ShortName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'ShortName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }
            }
        }
    }
}
