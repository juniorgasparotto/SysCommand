namespace SysCommand.Parsing
{
    /// <summary>
    /// Represents a piece of the command line.
    /// </summary>
    public class ArgumentRaw
    {
        /// <summary>
        /// Position in command line
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Argument name if exists
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value without scapes
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Argument format
        /// </summary>
        public ArgumentFormat Format { get; set; }

        /// <summary>
        /// Delimiter format if exists
        /// </summary>
        public string DelimiterArgument { get; set; }

        /// <summary>
        /// Value delimiter if exists
        /// </summary>
        public string DelimiterValueInName { get; set; }

        /// <summary>
        /// Value with scapes
        /// </summary>
        public string ValueRaw { get; set; }

        /// <summary>
        /// Check if the format is short
        /// </summary>
        public bool IsShortName
        {
            get
            {
                switch (this.Format)
                {
                    case ArgumentFormat.ShortNameAndHasValue:
                    case ArgumentFormat.ShortNameAndHasValueInName:
                    case ArgumentFormat.ShortNameAndNoValue:
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the format is long
        /// </summary>
        public bool IsLongName
        {
            get
            {
                switch (this.Format)
                {
                    case ArgumentFormat.LongNameAndHasValue:
                    case ArgumentFormat.LongNameAndHasValueInName:
                    case ArgumentFormat.LongNameAndNoValue:
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="index">Position in command line</param>
        /// <param name="name">Argument name</param>
        /// <param name="valueRaw">Value with scapes</param>
        /// <param name="value">Value without scapes</param>
        /// <param name="format">Argument format</param>
        /// <param name="delimiterArgument">Delimiter format if exists</param>
        /// <param name="delimiterValueInName">Value delimiter if exists</param>
        public ArgumentRaw(int index, string name, string valueRaw, string value, ArgumentFormat format, string delimiterArgument, string delimiterValueInName)
        {
            this.Index = index;
            this.Name = name;
            this.Value = value;
            this.ValueRaw = valueRaw;
            this.Format = format;
            this.DelimiterArgument = delimiterArgument;
            this.DelimiterValueInName = delimiterValueInName;
        }
        
        public override string ToString()
        {
            return "[" + this.Name + ", " + this.Value + "]";
        }

    }
}
