using System;

namespace SysCommand.Mapping
{
    public class ArgumentAttribute : Attribute
    {
        private int? position;
        private object defaultValue;

        public char ShortName { get; set; }
        public string LongName { get; set; }
        public bool IsRequired { get; set; }
        public string Help { get; set; }
        public bool HasDefaultValue { get; private set; }
        
        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                this.HasDefaultValue = true;
                defaultValue = value;
            }
        }

        public int Position
        {
            get
            {
                return this.position ?? 0;
            }
            set
            {
                this.position = value;
            }
        }

        public bool HasPosition
        {
            get { 
                return this.position == null ? false : true; 
            }
        }

        /// <summary>
        /// Displays the default value or the label "required" at the end of the sentence help.
        /// </summary>
        public bool ShowHelpComplement { get; set; }

        public ArgumentAttribute()
        {
            this.ShowHelpComplement = true;
        }
    }
}
