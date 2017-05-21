namespace SysCommand.Execution
{
    /// <summary>
    /// Execution state
    /// </summary>
    public enum ExecutionState
    {
        /// <summary>
        /// The execution was a success.
        /// </summary>
        Success,

        /// <summary>
        /// Execution failed
        /// </summary>
        HasError,

        /// <summary>
        /// The command was not found
        /// </summary>
        NotFound
    }
}
