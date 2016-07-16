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
    public enum EnumTest1
    {
        Enum1Value1 = 1,
        Enum1Value2 = 2,
        Enum1Value3 = 4,
        Enum1Value4 = 8
    }
}
