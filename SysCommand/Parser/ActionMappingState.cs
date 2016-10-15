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
    [Flags]
    public enum ActionMappingState
    {
        None = 0,
        Valid = 1,
        HasExtras = 2,
        NoArgumentsInMapAndInInput = 4,
        IsInvalid = 8
    }
}
