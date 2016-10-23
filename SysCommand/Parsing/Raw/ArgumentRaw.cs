namespace SysCommand.Parsing
{
    public class ArgumentRaw
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ArgumentFormat Format { get; set; }
        public string DelimiterArgument { get; set; }
        public string DelimiterValueInName { get; set; }
        public string ValueRaw { get; set; }

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

        public int Index { get; set; }
    }
}
