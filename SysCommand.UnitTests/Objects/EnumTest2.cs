using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SysCommand.Tests;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Newtonsoft.Json;
using TestUtils;

namespace SysCommand.UnitTests
{
    [Flags]
    public enum EnumTest2
    {
        Enum2Value1 = 1,
        Enum2Value2 = 2,
        Enum2Value3 = 4,
        Enum2Value4 = 8
    }
}
