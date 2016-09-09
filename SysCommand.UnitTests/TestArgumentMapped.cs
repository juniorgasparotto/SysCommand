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
    [TestClass]
    public class TestArgumentMapped
    {
        public TestArgumentMapped()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        private void TestArgsMappedAuto(string mapMethodName, string input, bool enablePositionedArgs, string testMethodName)
        {
            var obj = new ArgumentMappedTestObject();
            var method = obj.GetType().GetMethod(mapMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var maps = CommandParser.GetArgumentsMapsFromMethod(obj, method);
            this.TestArgsMappedAuto(maps, input, enablePositionedArgs, testMethodName);
        }

        private void TestArgsMappedAuto(object source, string input, bool enablePositionedArgs, bool onlyWithAttribute, string testMethodName)
        {
            var maps = CommandParser.GetArgumentsMapsFromProperties(source, onlyWithAttribute);
            this.TestArgsMappedAuto(maps, input, enablePositionedArgs, testMethodName);
        }

        private void TestArgsMappedAuto(IEnumerable<ArgumentMap> maps, string input, bool enablePositionedArgs, string testMethodName)
        {
            string testContext = null;
            var args = AppHelpers.CommandLineToArgs(input);
            var argsRaw = CommandParser.ParseArgumentRaw(args);
            var argsMappeds = CommandParser.ParseArgumentMapped(argsRaw, enablePositionedArgs, maps.ToArray());
            var objectTest = new { input, maps, argsMappeds };

            var jsonSerializeConfig = TestHelper.GetJsonConfig();
            jsonSerializeConfig.Converters.Add(new TestObjectJsonConverter());
            TestHelper.CompareObjects<TestArgumentMapped>(objectTest, testContext, testMethodName, jsonSerializeConfig);
        }

        [TestMethod]
        public void Method1And1Positioned3Named()
        {
            this.TestArgsMappedAuto("Method1", "value-positioned -a value1 -b value2 -c value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3Named1WithoutValue()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 -b -c value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3ArgsIn1()
        {
            this.TestArgsMappedAuto("Method1", "-abc value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInLast()
        {
            this.TestArgsMappedAuto("Method1", "-b value1 -c value3 value2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1Named2Positioned()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 value2 value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInMiddle()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 value2 -c value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedInLast2()
        {
            this.TestArgsMappedAuto("Method1", "-a value1 -b value2 value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1NamedInMiddleAnd2Positioned()
        {
            this.TestArgsMappedAuto("Method1", "value1 -b value2 value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And1NamedInFirstAnd2Named()
        {
            this.TestArgsMappedAuto("Method1", "value1 -b value2 -c value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Positioned1NamedInLast()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2 -c value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositioned()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2 value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2PositionedWithout1Arg()
        {
            this.TestArgsMappedAuto("Method1", "value1 value2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2PositionedWithQuoteInContent()
        {
            this.TestArgsMappedAuto("Method1", "value1 \"\\\"quote in content\\\"\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndArgsFormatScapeds()
        {
            this.TestArgsMappedAuto(@"Method1", @"\--value1 \-value2 \/value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndScapeArgsAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"\\--value1 \\-value2 \\/value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndAllPositionedAndArgsFormatAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"- \- /", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And2Named1PositionedAndAllArgsHasFormatAsValue()
        {
            this.TestArgsMappedAuto("Method1", @"-a - -b \- \/", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1And3NamedWithScapes()
        {
            this.TestArgsMappedAuto("Method1", @"-a \--value1 -b \/ -c --", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method1AndBackslashAsValues()
        {
            this.TestArgsMappedAuto("Method1", @"-a \ \ -c \", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsTrue()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 true", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAs0()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 0", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsPLUS()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 +", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndBooleanAsFLAG()
        {
            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndDecimalAndIntAsNegativeAndBooleanInvalid()
        {
            this.TestArgsMappedAuto("Method2", "--value1 -10.12 --value2 -10 --value3 -1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndLastArgPositionedAndInvalidAndOthersAsDefault()
        {
            this.TestArgsMappedAuto("Method2", "--value1 --value2 invalid", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndPositionedAndFirstNegative()
        {
            this.TestArgsMappedAuto("Method2", "-1 1 1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method2AndFirstNegativeAndTheMiddleArgsWithPLUS()
        {
            this.TestArgsMappedAuto("Method2", "-1.10 +100 true", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method3AndSimpleTestIncludeDatetime()
        {
            this.TestArgsMappedAuto("Method3", "--value1 10.1223 --value2 10.2344 --date \"12-22/1879 12:10:10.111\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method3AndPositionedTestIncludeNegativeValueAndDatetime()
        {
            this.TestArgsMappedAuto("Method3", "-2000 -10.2222 \"01-01/2016 12:10:10.111\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method4And2ArgsAsFLAGAnd1ArgsBooleanWithNEGATIVEFormat()
        {
            this.TestArgsMappedAuto("Method4", "-ab --value-", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method4AndPositionedAllAsTrueValue()
        {
            this.TestArgsMappedAuto("Method4", "true - +", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5And2NamedWithDefaultsAnd1PositionedAndInvalid()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 --enum2 invalid", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5And3NamedWithDefault()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 --enum3 --enum4", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndInvalidEnumValueInMiddleAndShiftedTheRestByPosition()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndInvalidEnumValueInMiddleButThePositionedFeaturedIsOFF()
        {
            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositioned()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 value1 1 2 3 4 5", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositioned2()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 b 1 2 3 4 5 Value4", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method5AndAllPositionedExceptTheLast()
        {
            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 --enum3 value1 1 2 3 4 5", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method6And3ListIntsDecimalsAndStrings()
        {
            this.TestArgsMappedAuto("Method6", "--lst 1 2 3 4 --lst2 10 10.1 10.2 10.333 --lst3 a b c \"1 2 3 4\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method6And3ListIntsDecimalsAndStringsPositioned()
        {
            this.TestArgsMappedAuto("Method6", "1 2 3 4 10 10.1 10.2 10.333 a b c \"\\\"quotes in content\\\"\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method7And2ListIntsAndChars()
        {
            this.TestArgsMappedAuto("Method7", "--lst 1 2 3 4 --lst2 a b c abc", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method7And2ListIntsAndCharsPositioned()
        {
            this.TestArgsMappedAuto("Method7", "-0.1 2 0.1000 4 a b c", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And5NamedNullableWithValues()
        {
            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2 --v3 10 --v4 1 0 1 0 --v5 a b c", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And5NamedNullableWithoutValuesSettingAsNull()
        {
            this.TestArgsMappedAuto("Method8", "--v1 --v2 --v3 --v4 --v5", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8And2NamedAnd3Missing()
        {
            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2.10", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method8AndAllPositionedIncludeArrayOfBytesAndListOfChars()
        {
            this.TestArgsMappedAuto("Method8", "-10 -10.10 2000 0 1 0 1 0 1 a b c", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithValue()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithoutValue()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And1NamedUnsuporttedTypeAnd1Missing()
        {
            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method9And2PositionedUnsuporttedTypeAndDefaultValueWithValue()
        {
            this.TestArgsMappedAuto("Method9", "abc 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And2Named1ListOfStringAnd1Int()
        {
            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c --v2 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And1Named1ListOfStringAnd1PositionedInt()
        {
            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And2Positioned1ListOfStringAnd1Int()
        {
            this.TestArgsMappedAuto("Method10", "\"abc\" a b c 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10And1Positioned1ListOfStringAnd1NamedInt()
        {
            this.TestArgsMappedAuto("Method10", "\"abc\" a b c --v2 20", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants()
        {
            this.TestArgsMappedAuto("Method10", "\\--value1 --=a --- --: -: -= -/ /= /- ", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants2()
        {
            this.TestArgsMappedAuto("Method10", "--v2 100000 lst1 lst2 --\"value1\":0 --value2=0 ", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants3()
        {
            this.TestArgsMappedAuto("Method10", "--2000 /0 --value=value--value=junior --value=\"value\" --v2 100000", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void Method10AndSeveralValuesVariants4()
        {
            this.TestArgsMappedAuto("Method10", "-abc=+ -def= + -1=0 \"--bla\" -l:=+\\\"quote\\\" --value=\"--value\"", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void MethodInvalidShortName()
        {
            try
            {
                this.TestArgsMappedAuto("Method11", "1234", true, TestHelper.GetCurrentMethodName());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The map 'v1' has 'ShortName' invalid in method or class 'Method11'");
            }
        }

        [TestMethod]
        public void MethodInvalidLongNameEmpty()
        {
            try
            {
                this.TestArgsMappedAuto("Method12", "1234", true, TestHelper.GetCurrentMethodName());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The map 'v1' has 'LongName' invalid in method or class 'Method12'");
            }
        }

        [TestMethod]
        public void MethodInvalidShortNameEmpty()
        {
            try
            {
                this.TestArgsMappedAuto("Method13", "1234", true, TestHelper.GetCurrentMethodName());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The map 'v1' has 'ShortName' invalid in method or class 'Method13'");
            }
        }

        [TestMethod]
        public void MethodDuplicateShortName()
        {
            try
            {
                this.TestArgsMappedAuto("Method14", "1234", true, TestHelper.GetCurrentMethodName());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The map 'v1' has the same 'ShortName' on the map 'v2' in method or class 'Method14'");
            }
        }

        [TestMethod]
        public void MethodDuplicateLongName()
        {
            try
            {
                this.TestArgsMappedAuto("Method15", "1234", true, TestHelper.GetCurrentMethodName());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The map 'v1' has the same 'LogName' on the map 'v2' in method or class 'Method15'");
            }
        }

        [TestMethod]
        public void MethodLongNameWith1Char1()
        {
            this.TestArgsMappedAuto("Method16", "--v value1 -A value2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void MethodLongNameWith1Char2()
        {
            this.TestArgsMappedAuto("Method16", "-v value1 --value-UPPER value2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void MethodOrdered()
        {
            this.TestArgsMappedAuto("Method17", "--value-order-three value1 --value-order-two value2 --value-order-one value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void MethodOrdered2()
        {
            this.TestArgsMappedAuto("Method17", "value1 value2 value3", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void InstanceOrdered()
        {
            this.TestArgsMappedAuto(new ArgumentMappedTestObject(), "--prop1 value1 --prop2 value2", true, false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void InstanceOrdered2()
        {
            this.TestArgsMappedAuto(new ArgumentMappedTestObject(), "value2 value1", true, false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void InstanceAllProperties()
        {
            this.TestArgsMappedAuto(new ArgumentMappedTestObject(), "--prop1 value1 --prop2 value2 --p value3 --prop4 value4 --prop5 value5", true, false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void InstanceAllPropertiesOnlyWithAttributes()
        {
            this.TestArgsMappedAuto(new ArgumentMappedTestObject(), "--prop1 value1 --prop2 value2 --p value3 --prop4 value4 --prop5 value5", true, true, TestHelper.GetCurrentMethodName());
        }
    }
}
