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
    public enum EnumTestNoFlag
    {
        Value1 = 'a',
        Value2 = 'b',
        Value3 = 'c',
        Value4 = 'd',
    }
}
