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

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestParseArguments
    {
        JsonSerializerSettings jsonSerializeConfig;

        public TestParseArguments()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            this.jsonSerializeConfig = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            this.jsonSerializeConfig.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            this.jsonSerializeConfig.TypeNameHandling = TypeNameHandling.None;
        }

        private string Join(string separator, params string[] values)
        {
            return string.Join(separator, values);
        }

        private string GetValidTestFileName(string testContext, string fileName)
        {
            var fileNameFormat = @"Tests\{0}-{1}\valid-{2}.json";
            fileName = string.Format(fileNameFormat, this.GetType().Name, testContext, fileName);
            return Path.Combine(@"..\..\", fileName);
        }

        private string GetUncheckedTestFileName(string testContext, string fileName)
        {
            var fileNameFormat = @"Tests\{0}-{1}\unchecked-{2}.json";
            fileName = string.Format(fileNameFormat, this.GetType().Name, testContext, fileName);
            return Path.Combine(@"..\..\", fileName);
        }

        private void SaveUncheckedFileIfValidNotExists<T>(T obj, string testContext, string fileName)
        {
            var validFileName = this.GetValidTestFileName(testContext, fileName);
            var uncheckedFileName = this.GetUncheckedTestFileName(testContext, fileName);
            if (FileHelper.GetObjectFromFileJson<T>(validFileName, jsonSerializeConfig) == null)
                FileHelper.SaveObjectToFileJson(obj, uncheckedFileName, jsonSerializeConfig);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        private void TestArgsMappedAuto(string mapMethodName, string input, bool enablePositionedArgs, string testMethodName)
        {
            var testContext = "Maps";

            var method = this.GetType().GetMethod(mapMethodName);
            var args = AppHelpers.CommandLineToArgs(input);
            var maps = ArgumentsParser.GetArgumentMaps(method);
            var argsRaw = ArgumentsParser.Parser(args);
            var argsMappeds = ArgumentsParser.Parser(argsRaw, enablePositionedArgs, maps.ToArray());
            var objectTest = new { input, maps, argsMappeds };

            // add if not exists, and the first add must be correct
            this.SaveUncheckedFileIfValidNotExists<dynamic>(objectTest, testContext, testMethodName);

            var outputTest = FileHelper.GetContentJsonFromObject(objectTest, jsonSerializeConfig);
            var outputCorrect = FileHelper.GetContentFromFile(this.GetValidTestFileName(testContext, testMethodName));
            Assert.IsTrue(outputTest == outputCorrect, string.Format("Error is test file '{0}'", testMethodName));
        }

        [TestMethod]
        public void Method1And1Positioned3Named()
        {
            this.TestArgsMappedAuto("Method1", "value-positioned -a value1 -b value2 -c value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3Named1WithoutValue()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 -b -c value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3ArgsIn1()
        {
            this.TestArgsMappedAuto("Method1", "-abc value1", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInLast()
        {
            this.TestArgsMappedAuto("Method1", "-b value1 -c value3 value2", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1Named2Positioned()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 value2 value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInMiddle()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 value2 -c value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInLast2()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 -b value2 value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1NamedInMiddleAnd2Positioned()
        {
            this.TestArgsMappedAuto("Method1", "value1 -b value2 value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1NamedInFirstAnd2Named()
        {
            this.TestArgsMappedAuto("Method1", "value1 -b value2 -c value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Positioned1NamedInLast()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2 -c value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositioned()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2 value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2PositionedWithout1Arg()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2PositionedWithQuoteInContent()
        {
            this.TestArgsMappedAuto("Method1", "value1 \"\\\"quote in content\\\"\"", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndArgsFormatScapeds()
        {
            this.TestArgsMappedAuto(@"Method1", @"\--value1 \-value2 \/value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndScapeArgsAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"\\--value1 \\-value2 \\/value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndArgsFormatAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"- \- /", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedAndAllArgsHasFormatAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"-a - -b \- \/", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3NamedWithScapes()
        {
            this.TestArgsMappedAuto("Method1", @"-a \--value1 -b \/ -c --", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndBackslashAsValues()
        {
            this.TestArgsMappedAuto("Method1", @"-a \ \ -c \", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsTrue()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 true", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAs0()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 0", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsPLUS()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 +", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsFLAG()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndDecimalAndIntAsNegativeAndBooleanInvalid()
        {
            this.TestArgsMappedAuto("Method2", "--value1 -10.12 --value2 -10 --value3 -1", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndLastArgPositionedAndInvalidAndOthersAsDefault()
        {
            this.TestArgsMappedAuto("Method2", "--value1 --value2 invalid", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndPositionedAndFirstNegative()
        {
            this.TestArgsMappedAuto("Method2", "-1 1 1", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndFirstNegativeAndTheMiddleArgsWithPLUS()
        {
            this.TestArgsMappedAuto("Method2", "-1.10 +100 true", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method3AndSimpleTestIncludeDatetime()
        {
            this.TestArgsMappedAuto("Method3", "--value1 10.1223 --value2 10.2344 --date \"12-22/1879 12:10:10.111\"", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method3AndPositionedTestIncludeNegativeValueAndDatetime()
        {
            this.TestArgsMappedAuto("Method3", "-2000 -10.2222 \"01-01/2016 12:10:10.111\"", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method4And2ArgsAsFLAGAnd1ArgsBooleanWithNEGATIVEFormat()
        {
            this.TestArgsMappedAuto("Method4", "-ab --value-", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method4AndPositionedAllAsTrueValue()
        {
            this.TestArgsMappedAuto("Method4", "true - +", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5And2NamedWithDefaultsAnd1PositionedAndInvalid()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 --enum2 invalid", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5And3NamedWithDefault()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 --enum3 --enum4", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndInvalidEnumValueInMiddleAndShiftedTheRestByPosition()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndInvalidEnumValueInMiddleButThePositionedFeaturedIsOFF()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", false, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositioned()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 value1 1 2 3 4 5", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositioned2()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 b 1 2 3 4 5 Value4", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositionedExceptTheLast()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 --enum3 value1 1 2 3 4 5", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method6And3ListIntsDecimalsAndStrings()
        {
            this.TestArgsMappedAuto("Method6", "--lst 1 2 3 4 --lst2 10 10.1 10.2 10.333 --lst3 a b c \"1 2 3 4\"", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method6And3ListIntsDecimalsAndStringsPositioned()
        {
            this.TestArgsMappedAuto("Method6", "1 2 3 4 10 10.1 10.2 10.333 a b c \"\\\"quotes in content\\\"\"", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method7And2ListIntsAndChars()
        {
            this.TestArgsMappedAuto("Method7", "--lst 1 2 3 4 --lst2 a b c abc", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method7And2ListIntsAndCharsPositioned()
        {
            this.TestArgsMappedAuto("Method7", "-0.1 2 0.1000 4 a b c", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And5NamedNullableWithValues()
        {
            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2 --v3 10 --v4 1 0 1 0 --v5 a b c", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And5NamedNullableWithoutValuesSettingAsNull()
        {
            this.TestArgsMappedAuto("Method8", "--v1 --v2 --v3 --v4 --v5", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And2NamedAnd3Missing()
        {
            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2.10", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8AndAllPositionedIncludeArrayOfBytesAndListOfChars()
        {
            this.TestArgsMappedAuto("Method8", "-10 -10.10 2000 0 1 0 1 0 1 a b c", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithValue()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithoutValue()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And1NamedUnsuporttedTypeAnd1Missing()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2PositionedUnsuporttedTypeAndDefaultValueWithValue()
        {
            this.TestArgsMappedAuto("Method9", "abc 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And2Named1ListOfStringAnd1Int()
        {
            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c --v2 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And1Named1ListOfStringAnd1PositionedInt()
        {
            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And2Positioned1ListOfStringAnd1Int()
        {
            this.TestArgsMappedAuto("Method10", "\"abc\" a b c 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And1Positioned1ListOfStringAnd1NamedInt()
        {
            this.TestArgsMappedAuto("Method10", "\"abc\" a b c --v2 20", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants()
        {
            this.TestArgsMappedAuto("Method10", "\\--value1 --=a --- --: -: -= -/ /= /- --2000 /0 --value=value--value=junior --value=\"value\" --v2 100000", true, this.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants()
        {
            this.TestArgsMappedAuto("Method10", "--v2 100000 lst1 lst2 --\"value1\":0 --value2=0 -abc=+ -def= + -1=0 \"--bla\" -l:=+\\\"quote\\\"", true, this.GetCurrentMethodName());
        }

        private string ToString(IEnumerable<ArgumentsParser.ArgumentRaw> items)
        {
            var output = "";
            foreach (var item in items)
            {
                output += "[" + item.Name + "]=[" + item.Value + "];";
            }
            return output;
        }
        
        [Flags]
        public enum EnumTest1
        {
            Enum1Value1 = 1,
            Enum1Value2 = 2,
            Enum1Value3 = 4,
            Enum1Value4 = 8
        }

        [Flags]
        public enum EnumTest2
        {
            Enum2Value1 = 1,
            Enum2Value2 = 2,
            Enum2Value3 = 4,
            Enum2Value4 = 8
        }

        public enum EnumTestNoFlag
        {
            Value1 = 'a',
            Value2 = 'b',
            Value3 = 'c',
            Value4 = 'd',
        }

        public enum EnumTestNoFlag2
        {
            Value1,
            Value2,
            Value3,
            Value4,
        }

        public class Class1 
        {
            public string Prop1 { get; set;}
        }

        public void Method1(string a, string b, string c) { }
        public void Method2(decimal value1, int value2, bool value3) { }
        public void Method3(double value1, float value2, DateTime date) { }
        public void Method4(bool a, bool b, bool value) { }
        public void Method5(EnumTest1? enum1, EnumTest2 enum2, EnumTestNoFlag enum3, int[] lst, EnumTestNoFlag2 enum4) { }
        public void Method6(List<double> lst, List<decimal> lst2, List<string> lst3) { }
        public void Method7(List<float> lst, List<char> lst2) { }

        public void Method8(int? v1, decimal? v2, long? v3, byte?[] v4, List<char?> v5) { }
        public void Method9(Class1 unsuporttedType, int defaultValue = 10) { }
        public void Method10(string[] v1, int v2) { }
    }
}
