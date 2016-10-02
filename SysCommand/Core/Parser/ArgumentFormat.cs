using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace SysCommand.Parser
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
