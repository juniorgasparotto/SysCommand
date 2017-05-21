namespace SysCommand.Execution
{
    /// <summary>
    /// Exposes a generic result model for members as properties or methods.
    /// </summary>
    public interface IMemberResult
    {
        /// <summary>
        /// Member name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Member Target  (owner class)
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Member value
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Checks if member was invoked
        /// </summary>
        bool IsInvoked { get; set; }

        /// <summary>
        /// Invoke member
        /// </summary>
        void Invoke();
    }
}
