using System.Collections.Generic;
using System.Linq;
using System;
using SysCommand.Mapping;
using System.Text;
using SysCommand.Helpers;

namespace SysCommand.Parsing
{
    /// <summary>
    /// Represents a parsed argument
    /// </summary>
    public class ArgumentParsed
    {
        private List<ArgumentRaw> allRaw;

        /// <summary>
        /// Argument name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Original value
        /// </summary>
        public object ValueParsed { get; set; }

        /// <summary>
        /// Value final
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Check if this value is unsuportted
        /// </summary>
        public bool HasUnsuporttedType { get; set; }

        /// <summary>
        /// Check if value is invalid for type
        /// </summary>
        public bool HasInvalidInput { get; set; }

        /// <summary>
        /// Argument type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Parsing type
        /// </summary>
        public ArgumentParsedType ParsingType { get; set; }

        /// <summary>
        /// Argument state
        /// </summary>
        public ArgumentParsedState ParsingStates { get; set; }

        /// <summary>
        /// Argument map
        /// </summary>
        public ArgumentMap Map { get; set; }

        /// <summary>
        /// Original value (string)
        /// </summary>
        public string Raw
        {
            get
            {
                return GetValueRaw(this.allRaw.Select(f => f.ValueRaw).ToArray());
            }
        }

        /// <summary>
        /// All raw that composes the final value
        /// </summary>
        public IEnumerable<ArgumentRaw> AllRaw
        {
            get
            {
                return allRaw;
            }
        }

        /// <summary>
        /// Check if argument was mapped
        /// </summary>
        public bool IsMapped
        {
            get
            {
                if (this.ParsingType.In(ArgumentParsedType.Name, ArgumentParsedType.Position))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Check if argument was mapped or has default value
        /// </summary>
        public bool IsMappedOrHasDefaultValue
        {
            get
            {
                if (this.IsMapped || this.ParsingType == ArgumentParsedType.DefaultValue)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="valueParsed">Original value</param>
        /// <param name="value">Final value</param>
        /// <param name="type">Argument type</param>
        /// <param name="map">Argument map</param>
        public ArgumentParsed(string name, string valueParsed, string value, Type type, ArgumentMap map)
        {
            this.Name = name;
            this.Value = value;
            this.ValueParsed = valueParsed;
            this.Type = type;
            this.Map = map;
            this.allRaw = new List<ArgumentRaw>();
        }

        /// <summary>
        /// Add raw for this argument value
        /// </summary>
        /// <param name="raw">ArgumentRaw value</param>
        public void AddRaw(ArgumentRaw raw)
        {
            this.allRaw.Add(raw);
        }

        /// <summary>
        /// Returns the format of the argument name in the input.
        /// </summary>
        /// <returns></returns>
        public string GetArgumentNameInputted()
        {
            string argumentNameInputted = null;
            var rawNamed = this.AllRaw.FirstOrDefault();
            if (rawNamed != null && rawNamed.Name != null)
                argumentNameInputted = rawNamed.DelimiterArgument + rawNamed.Name;
            else if (this.Map != null && this.Map.LongName != null)
                argumentNameInputted = "--" + this.Map.LongName;
            else if (this.Map != null && this.Map.ShortName != null)
                argumentNameInputted = "-" + this.Map.ShortName;

            return argumentNameInputted;
        }

        public override string ToString()
        {
            return "[" + this.Name + ", " + this.Value + "]";
        }

        /// <summary>
        /// Get all values as string
        /// </summary>
        /// <param name="values">List of values</param>
        /// <returns>Value as string</returns>
        public static string GetValueRaw(params string[] values)
        {
            var hasString = false;
            var strBuilder = new StringBuilder();
            foreach (var v in values)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    var value = v.Replace("\"", "\\\"");

                    hasString = true;
                    if (strBuilder.Length > 0)
                        strBuilder.Append(" ");

                    if (value.Contains(" "))
                    {
                        strBuilder.Append("\"");
                        strBuilder.Append(value);
                        strBuilder.Append("\"");
                    }
                    else
                    {
                        strBuilder.Append(value);
                    }
                }
            }

            return hasString ? strBuilder.ToString() : null;
        }

    }
}
