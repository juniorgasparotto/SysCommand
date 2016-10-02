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
    [Flags]
    public enum ArgumentMappingState
    {
        None = 0,
        ArgumentAlreadyBeenSet = 1,
        ArgumentNotExistsByName = 2,
        ArgumentNotExistsByValue = 4,
        ArgumentIsRequired = 8,
        ArgumentHasInvalidInput = 16,
        ArgumentHasUnsupportedType = 32,
        IsInvalid = 64,
        Valid = 128
    }
}
