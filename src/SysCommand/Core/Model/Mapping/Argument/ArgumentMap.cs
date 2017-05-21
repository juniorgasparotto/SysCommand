using System;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Map of argument
    /// </summary>
    public class ArgumentMap
    {
        /// <summary>
        /// Represent the member name
        /// </summary>
        public string MapName { get; private set; }

        /// <summary>
        /// Argument long name
        /// </summary>
        public string LongName { get; private set; }

        /// <summary>
        /// Argument short name
        /// </summary>
        public char? ShortName { get; private set; }

        /// <summary>
        /// Argument type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Check if property has default value
        /// </summary>
        public bool HasDefaultValue { get; private set; }

        /// <summary>
        /// Get default value
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// If true, this property isn't required
        /// </summary>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; private set; }

        /// <summary>
        /// Displays the default value or the label "required" at the end of the sentence help.
        /// </summary>
        public bool ShowHelpComplement { get; private set; }

        /// <summary>
        /// If the command accept positional arguments for properties, then you can set the positional order if you want.
        /// </summary>
        public int? Position { get; private set; }

        /// <summary>
        /// Target instance (owner class)
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Target member (PropertyInfo or ParameterInfo)
        /// </summary>
        public object TargetMember { get; private set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="target">Target instance (owner class)</param>
        /// <param name="targetMember">Target member (PropertyInfo or ParameterInfo)</param>
        /// <param name="mapName">Represent the member name</param>
        /// <param name="longName">Argument long name</param>
        /// <param name="shortName">Argument short name</param>
        /// <param name="type">Argument type</param>
        /// <param name="isOptional">If true, this property isn't required</param>
        /// <param name="hasDefaultValue">Check if property has default value</param>
        /// <param name="defaultValue">Get default value</param>
        /// <param name="helpText">Help text</param>
        /// <param name="showHelpComplement">Displays the default value or the label "required" at the end of the sentence help.</param>
        /// <param name="position">If the command accept positional arguments for properties, then you can set the positional order if you want.</param>
        public ArgumentMap(object target, object targetMember, string mapName, string longName, char? shortName, Type type, bool isOptional, bool hasDefaultValue, object defaultValue, string helpText, bool showHelpComplement, int? position)
        {
            this.Target = target;
            this.TargetMember = targetMember;
            this.MapName = mapName;
            this.LongName = longName;
            this.ShortName = shortName;
            this.Type = type;
            this.IsOptional = isOptional;
            this.HasDefaultValue = hasDefaultValue;
            this.DefaultValue = defaultValue;
            this.HelpText = helpText;
            this.ShowHelpComplement = showHelpComplement;
            this.Position = position;
        }

        public override string ToString()
        {
            return "[" + this.MapName + ", " + this.Type + "]";
        }
    }
}
