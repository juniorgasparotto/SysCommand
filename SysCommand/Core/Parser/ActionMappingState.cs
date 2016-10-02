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
        NoArgumentsInMapAndInInput = 1,
        AllArgumentsAreMapped = 2,
        IsInvalid = 4,
        Valid = 8
        //AmountOfMappedIsBiggerThenMaps = 8,
        //AmountOfMappedIsLessThanMaps = 2,
        //AnyArgumentHasNoInput = 1,
        //AnyArgumentNotMapped = 2,
        //AnyArgumentHasDefaultValue = 4,
        //AnyArgumentIsPositional = 8,
        //AnyArgumentIsNamed = 16,
        //AnyArgumentHasUnsuporttedType = 32,
        //AnyArgumentHasInvalidInput = 64,        
        //IsValid = 512,
    }
}
