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

namespace SysCommand
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
