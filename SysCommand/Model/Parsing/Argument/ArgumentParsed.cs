using System.Collections.Generic;
using System.Linq;
using System;
using SysCommand.Mapping;
using System.Text;
using SysCommand.Utils;

namespace SysCommand.Parsing
{
    public class ArgumentParsed
    {
        private List<ArgumentRaw> allRaw;

        public string Name { get; set; }
        public object ValueParsed { get; set; }
        public object Value { get; set; }
        public bool HasUnsuporttedType { get; set; }
        public bool HasInvalidInput { get; set; }
        public Type Type { get; set; }
        public ArgumentParsedType ParsingType { get; set; }
        public ArgumentParsedState ParsingStates { get; set; }
        public ArgumentMap Map { get; set; }

        public string Raw
        {
            get
            {
                return GetValueRaw(this.allRaw.Select(f => f.ValueRaw).ToArray());
            }
        }

        public IEnumerable<ArgumentRaw> AllRaw
        {
            get
            {
                return allRaw;
            }
        }

        public bool IsMapped
        {
            get
            {
                if (this.ParsingType.In(ArgumentParsedType.Name, ArgumentParsedType.Position))
                    return true;
                return false;
            }
        }

        public bool IsMappedOrHasDefaultValue
        {
            get
            {
                if (this.IsMapped || this.ParsingType == ArgumentParsedType.DefaultValue)
                    return true;
                return false;
            }
        }

        public ArgumentParsed(string name, string valueParsed, string value, Type type, ArgumentMap map)
        {
            this.Name = name;
            this.Value = value;
            this.ValueParsed = valueParsed;
            this.Type = type;
            this.Map = map;
            this.allRaw = new List<ArgumentRaw>();
        }

        public void AddRaw(ArgumentRaw raw)
        {
            this.allRaw.Add(raw);
        }

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
