namespace SysCommand.Parser
{
    // <summary>
    // </summary>
    // Map: a, b, c, d, e, f = 1
    // Input: 1 -b -c -d - j
    // Result expected:
    //  1: Position
    // -b: Name
    // -c: Name
    // -d: Name
    // -e: HasNoInput
    // -j: NotMapped
    // -f: DefaultValue
    public enum ArgumentMappingType
    {
        Name,
        Position,
        DefaultValue,
        HasNoInput,
        NotMapped,
    }
}
