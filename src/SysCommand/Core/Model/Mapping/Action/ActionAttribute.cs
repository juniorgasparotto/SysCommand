using System;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Settings for an action
    /// </summary>
    public class ActionAttribute : Attribute
    {
        /// <summary>
        /// Action name. If it is null, a default name is used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines whether this action can have prefix of the command
        /// </summary>
        public bool UsePrefix { get; set; }

        /// <summary>
        /// Ignores this method so it is not considered an action.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// If true, this method may have its name omitted from the user input.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Enable positional inputs for your parameters.
        /// </summary>
        public bool EnablePositionalArgs { get; set; }

        /// <summary>
        /// Help text
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public ActionAttribute()
        {
            this.UsePrefix = true;
            this.EnablePositionalArgs = true;
        }
    }
}
