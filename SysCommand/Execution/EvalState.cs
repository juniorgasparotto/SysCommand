using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public enum EvalState
    {
        Success,
        HasInvalidAction,
        HasInvalidArgument,
        NotFound,
        //ArgsIsEmpty,
        //UnexpectedError,
    }
}
