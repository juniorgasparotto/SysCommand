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
    public class ArgumentMappedTestObject
    {
        [Argument(Position = 1)]
        public string Prop1 { get; set; }

        [Argument(Position = 0)]
        public string Prop2 { get; set; }

        [Argument(DefaultValue = "default", IsRequired = true, Help = "Help", ShowHelpComplement = true, ShortName = 'p', LongName = "p")]
        public string Prop3 { get; set; }

        public string Prop4 { get; set; }

        [Argument]
        public string Prop5 { get; set; }

        public void Method1(string a, string b, string c) { }
        public void Method2(decimal value1, int value2, bool value3) { }
        public void Method3(double value1, float value2, DateTime date) { }
        public void Method4(bool a, bool b, bool value) { }
        public void Method5(EnumTest1? enum1, EnumTest2 enum2, EnumTestNoFlag enum3, int[] lst, EnumTestNoFlag2 enum4) { }
        public void Method6(List<double> lst, List<decimal> lst2, List<string> lst3) { }
        public void Method7(List<float> lst, List<char> lst2) { }

        public void Method8(int? v1, decimal? v2, long? v3, byte?[] v4, List<char?> v5) { }
        public void Method9(ArgumentMappedTestObject unsuporttedType, int defaultValue = 10) { }
        public void Method10(string[] v1, int v2) { }

        // MethodInvalidName
        public void Method11(
            [Argument(LongName = "value", ShortName = '2', ShowHelpComplement = true, Help = "My help")]
            string v1, int v2) { }

        // MethodInvalidName empty longName
        public void Method12(
            [Argument(LongName = "", ShortName = 'a', ShowHelpComplement = true, Help = "My help")]
            string v1, int v2) { }

        // MethodInvalidName empty shortName
        public void Method13(
            [Argument(LongName = "a", ShortName = ' ', ShowHelpComplement = true, Help = "My help")]
            string v1, int v2) { }

        //MethodDuplicateName
        public void Method14(
            [Argument(LongName = "value", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
            string v1,
            [Argument(LongName = "value2", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
            int v2) { }

        //MethodDuplicateName
        public void Method15(
            [Argument(LongName = "value", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
            string v1,
            [Argument(LongName = "value", ShortName = 'a', ShowHelpComplement = true, Help = "My help")]
            int v2) { }

        //MethodLongNameWith1Char
        public void Method16(
            [Argument(LongName = "v", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
            string value,
            [Argument(LongName = "value-UPPER", ShortName = 'A', ShowHelpComplement = true, Help = "My help")]
            bool flag) { }

        //MethodOrdered
        public void Method17(
            string valueOrderThree,
            [Argument(Position = 0)]
            string valueOrderOne,
            [Argument(Position = 1)]
            string valueOrderTWO) { }
    }
}
