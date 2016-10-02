using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace SysCommand.Parser
{
    public class ArgumentMapped
    {
        private List<ArgumentRaw> allRaw;

        public string Name { get; set; }
        public object ValueParsed { get; set; }
        public object Value { get; set; }
        public bool HasUnsuporttedType { get; set; }
        public bool HasInvalidInput { get; set; }
        public Type Type { get; set; }
        public ArgumentMappingType MappingType { get; set; }
        public ArgumentMappingState MappingStates { get; set; }
        public ArgumentMap Map { get; set; }

        public string Raw
        {
            get
            {
                return CommandParser.GetValueRaw(this.allRaw.Select(f => f.ValueRaw).ToArray());
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
                if (this.MappingType.In(ArgumentMappingType.Name, ArgumentMappingType.Position))
                    return true;
                return false;
            }
        }

        public bool IsMappedOrHasDefaultValue
        {
            get
            {
                if (this.IsMapped || this.MappingType == ArgumentMappingType.DefaultValue)
                    return true;
                return false;
            }
        }

        public ArgumentMapped(string name, string valueParsed, string value, Type type, ArgumentMap map)
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

    }
}
