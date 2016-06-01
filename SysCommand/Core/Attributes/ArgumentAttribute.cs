using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SysCommand
{
    public class ArgumentAttribute : Attribute
    {
        public char ShortName { get; set; }
        public string LongName { get; set; }
        public bool IsRequired { get; set; }
        public string Help { get; set; }
        public object Default { get; set; }

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
