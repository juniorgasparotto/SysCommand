using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public interface IMember
    {
        string Name { get; }
        string Alias { get; }
        object Source { get; }
        object Value { get; }
        bool IsInvoked { get; }
        //int InvokePriority { get; }
        void Invoke();
    }
}
