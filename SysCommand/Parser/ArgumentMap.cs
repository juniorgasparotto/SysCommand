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
    public class ArgumentMap
    {
        public string MapName { get; private set; }
        public string LongName { get; private set; }
        public char? ShortName { get; private set; }
        public Type Type { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public object DefaultValue { get; private set; }
        public bool IsOptional { get; private set; }
        public string HelpText { get; private set; }
        public bool ShowHelpComplement { get; private set; }
        public int? Position { get; private set; }
        public object Source { get; private set; }
        public object PropertyOrParameter { get; private set; }

        public ArgumentMap(object source, object propertyOrParameter, string mapName, string longName, char? shortName, Type type, bool isOptional, bool hasDefaultValue, object defaultValue, string helpText, bool showHelpComplement, int? position)
        {
            this.Source = source;
            this.PropertyOrParameter = propertyOrParameter;
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
