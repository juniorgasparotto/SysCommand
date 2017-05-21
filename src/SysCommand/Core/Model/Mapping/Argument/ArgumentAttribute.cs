using System;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Settings for an property
    /// </summary>
    public class ArgumentAttribute : Attribute
    {
        private int? position;
        private object defaultValue;

        /// <summary>
        /// Argument short name
        /// </summary>
        public char ShortName { get; set; }

        /// <summary>
        /// Argument long name
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// If true, this property will be required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Help text
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Check if property has default value
        /// </summary>
        public bool HasDefaultValue { get; private set; }
        
        /// <summary>
        /// Get default value
        /// </summary>
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

        /// <summary>
        /// If the command accept positional arguments for properties, then you can set the positional order if you want.
        /// </summary>
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

        /// <summary>
        /// Check if position has value
        /// </summary>
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

        /// <summary>
        /// Initialize
        /// </summary>
        public ArgumentAttribute()
        {
            this.ShowHelpComplement = true;
        }
    }
}
