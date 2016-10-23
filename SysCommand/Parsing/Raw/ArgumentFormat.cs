namespace SysCommand.Parsing
{
    /// <summary>
    /// Input: 1 -a -b value1 -c+ --long --long2 value2 /long3:+
    /// Result expected:
    ///  1: Unnamed
    /// -a: ShortNameAndNoValue
    /// -b: ShortNameAndHasValue
    /// -c: ShortNameAndHasValueInName
    /// --long: LongNameAndNoValue
    /// --long2: LongNameAndHasValue
    /// --long3: LongNameAndHasValueInName
    /// </summary>
    public enum ArgumentFormat
    {
        Unnamed,
        ShortNameAndNoValue,
        ShortNameAndHasValue,
        ShortNameAndHasValueInName,
        LongNameAndNoValue,
        LongNameAndHasValue,
        LongNameAndHasValueInName,
    }
}
