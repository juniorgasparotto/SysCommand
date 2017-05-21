namespace SysCommand.Parsing
{
    /// <summary>
    /// Parsed argument type
    /// Map: a, b, c, d, e, f = 1
    /// Input: 1 -b -c -d - j
    /// Result expected:
    ///  1: Position
    /// -b: Name
    /// -c: Name
    /// -d: Name
    /// -e: HasNoInput
    /// -j: NotMapped
    /// -f: DefaultValue
    /// </summary>
    public enum ArgumentParsedType
    {
        Name,
        Position,
        DefaultValue,
        HasNoInput,
        NotMapped,
    }
}
