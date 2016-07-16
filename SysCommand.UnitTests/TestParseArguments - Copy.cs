//using System;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.IO;
//using SysCommand.Tests;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Globalization;
//using System.Collections;
//using System.Runtime.CompilerServices;
//using System.Diagnostics;
//using Newtonsoft.Json;

//namespace SysCommand.UnitTests
//{
//    [TestClass]
//    public class TestParseArguments
//    {
//        JsonSerializerSettings jsonSerializeConfig;

//        public TestParseArguments()
//        {
//            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
//            this.jsonSerializeConfig = new JsonSerializerSettings
//            {
//                TypeNameHandling = TypeNameHandling.Auto,
//                Formatting = Formatting.Indented
//            };

//            this.jsonSerializeConfig.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
//            this.jsonSerializeConfig.TypeNameHandling = TypeNameHandling.None;
//        }

//        private string Join(string separator, params string[] values)
//        {
//            return string.Join(separator, values);
//        }

//        private string GetValidTestFileName(string testContext, string fileName)
//        {
//            var fileNameFormat = @"Tests\{0}-{1}\valid-{2}.json";
//            fileName = string.Format(fileNameFormat, this.GetType().Name, testContext, fileName);
//            return Path.Combine(@"..\..\", fileName);
//        }

//        private string GetInvalidTestFileName(string testContext, string fileName)
//        {
//            var fileNameFormat = @"Tests\{0}-{1}\valid-{2}.json.invalid";
//            fileName = string.Format(fileNameFormat, this.GetType().Name, testContext, fileName);
//            return Path.Combine(@"..\..\", fileName);
//        }

//        private string GetUncheckedTestFileName(string testContext, string fileName)
//        {
//            var fileNameFormat = @"Tests\{0}-{1}\unchecked-{2}.json";
//            fileName = string.Format(fileNameFormat, this.GetType().Name, testContext, fileName);
//            return Path.Combine(@"..\..\", fileName);
//        }

//        private void SaveUncheckedFileIfValidNotExists<T>(T obj, string testContext, string fileName)
//        {
//            var validFileName = this.GetValidTestFileName(testContext, fileName);
//            var uncheckedFileName = this.GetUncheckedTestFileName(testContext, fileName);
//            if (!FileHelper.FileExists(validFileName))
//                FileHelper.SaveObjectToFileJson(obj, validFileName, jsonSerializeConfig);
//        }

//        private void SaveInvalidFileIfValidExists<T>(T obj, string testContext, string fileName)
//        {
//            var validFileName = this.GetValidTestFileName(testContext, fileName);
//            var invalidFileName = this.GetInvalidTestFileName(testContext, fileName);
//            if (FileHelper.FileExists(validFileName))
//                FileHelper.SaveObjectToFileJson(obj, invalidFileName, jsonSerializeConfig);
//        }

//        private void RemoveInvalidFile(string testContext, string fileName)
//        {
//            var invalidFileName = this.GetInvalidTestFileName(testContext, fileName);
//            if (FileHelper.FileExists(invalidFileName))
//                FileHelper.RemoveFile(invalidFileName);
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        public string GetCurrentMethodName()
//        {
//            StackTrace st = new StackTrace();
//            StackFrame sf = st.GetFrame(1);

//            return sf.GetMethod().Name;
//        }

//        private void TestArgsMappedAuto(string mapMethodName, string input, bool enablePositionedArgs, string testMethodName)
//        {
//            var method = this.GetType().GetMethod(mapMethodName);
//            var maps = ArgumentsParser.GetArgumentsMapsFromMethod(method);
//            this.TestArgsMappedAuto(maps, input, enablePositionedArgs, testMethodName);
//        }

//        private void TestArgsMappedAuto(Type typeInstance, string input, bool enablePositionedArgs, bool onlyWithAttribute, string testMethodName)
//        {
//            var maps = ArgumentsParser.GetArgumentsMapsFromProperties(typeInstance, onlyWithAttribute);
//            this.TestArgsMappedAuto(maps, input, enablePositionedArgs, testMethodName);
//        }

//        private void TestArgsMappedAuto(IEnumerable<ArgumentsParser.ArgumentMap> maps, string input, bool enablePositionedArgs, string testMethodName)
//        {
//            var testContext = "Maps";
//            var args = AppHelpers.CommandLineToArgs(input);
//            var argsRaw = ArgumentsParser.Parser(args);
//            var argsMappeds = ArgumentsParser.Parser(argsRaw, enablePositionedArgs, maps.ToArray());
//            var objectTest = new { input, maps, argsMappeds };

//            // add if not exists, and the first add must be correct
//            this.SaveUncheckedFileIfValidNotExists<dynamic>(objectTest, testContext, testMethodName);

//            var outputTest = FileHelper.GetContentJsonFromObject(objectTest, jsonSerializeConfig);
//            var outputCorrect = FileHelper.GetContentFromFile(this.GetValidTestFileName(testContext, testMethodName));
//            var test = outputTest == outputCorrect;

//            if (!test)
//                this.SaveInvalidFileIfValidExists<dynamic>(objectTest, testContext, testMethodName);
//            else
//                this.RemoveInvalidFile(testContext, testMethodName);

//            Assert.IsTrue(outputTest == outputCorrect, string.Format("Error is test file '{0}'", testMethodName));
//        }

//        private void TestActionMapped(IEnumerable<ArgumentsParser.ActionMap> actionMaps, string input, bool enableMultiAction, string testMethodName)
//        {
//            var testContext = "ActionMapped";
//            string[] args;
//            if (!string.IsNullOrWhiteSpace(input))
//                args = AppHelpers.CommandLineToArgs(input);
//            else 
//                args = new string[0];

//            var argsRaw = ArgumentsParser.Parser(args, actionMaps);
//            var actionsCallers = ArgumentsParser.GetActionsCallers(argsRaw, enableMultiAction, actionMaps.ToArray());
//            var objectTest = new { input, actionMaps, actionsCallers };

//            this.Test(objectTest, testContext, testMethodName);
//        }

//        private void Test(object objectTest, string testContext, string testMethodName)
//        {
//            //// add if not exists, and the first add must be correct
//            this.SaveUncheckedFileIfValidNotExists<dynamic>(objectTest, testContext, testMethodName);

//            var outputTest = FileHelper.GetContentJsonFromObject(objectTest, jsonSerializeConfig);
//            var outputCorrect = FileHelper.GetContentFromFile(this.GetValidTestFileName(testContext, testMethodName));
//            var test = outputTest == outputCorrect;

//            if (!test)
//                this.SaveInvalidFileIfValidExists<dynamic>(objectTest, testContext, testMethodName);
//            else
//                this.RemoveInvalidFile(testContext, testMethodName);

//            Assert.IsTrue(outputTest == outputCorrect, string.Format("Error is test file '{0}'", testMethodName));
//        }

//        [TestMethod]
//        public void Method1And1Positioned3Named()
//        {
//            this.TestArgsMappedAuto("Method1", "value-positioned -a value1 -b value2 -c value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And3Named1WithoutValue()
//        {
//            this.TestArgsMappedAuto("Method1", "-a value1 -b -c value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And3ArgsIn1()
//        {
//            this.TestArgsMappedAuto("Method1", "-abc value1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2Named1PositionedInLast()
//        {
//            this.TestArgsMappedAuto("Method1", "-b value1 -c value3 value2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And1Named2Positioned()
//        {
//            this.TestArgsMappedAuto("Method1", "-a value1 value2 value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2Named1PositionedInMiddle()
//        {
//            this.TestArgsMappedAuto("Method1", "-a value1 value2 -c value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2Named1PositionedInLast2()
//        {
//            this.TestArgsMappedAuto("Method1", "-a value1 -b value2 value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And1NamedInMiddleAnd2Positioned()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 -b value2 value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And1NamedInFirstAnd2Named()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 -b value2 -c value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2Positioned1NamedInLast()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 value2 -c value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1AndAllPositioned()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 value2 value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2PositionedWithout1Arg()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 value2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2PositionedWithQuoteInContent()
//        {
//            this.TestArgsMappedAuto("Method1", "value1 \"\\\"quote in content\\\"\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1AndAllPositionedAndArgsFormatScapeds()
//        {
//            this.TestArgsMappedAuto(@"Method1", @"\--value1 \-value2 \/value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1AndAllPositionedAndScapeArgsAsValue()
//        {
//            this.TestArgsMappedAuto("Method1", @"\\--value1 \\-value2 \\/value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1AndAllPositionedAndArgsFormatAsValue()
//        {
//            this.TestArgsMappedAuto("Method1", @"- \- /", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And2Named1PositionedAndAllArgsHasFormatAsValue()
//        {
//            this.TestArgsMappedAuto("Method1", @"-a - -b \- \/", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1And3NamedWithScapes()
//        {
//            this.TestArgsMappedAuto("Method1", @"-a \--value1 -b \/ -c --", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method1AndBackslashAsValues()
//        {
//            this.TestArgsMappedAuto("Method1", @"-a \ \ -c \", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndBooleanAsTrue()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 true", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndBooleanAs0()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 0", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndBooleanAsPLUS()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3 +", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndBooleanAsFLAG()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 10.12 --value2 10 --value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndDecimalAndIntAsNegativeAndBooleanInvalid()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 -10.12 --value2 -10 --value3 -1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndLastArgPositionedAndInvalidAndOthersAsDefault()
//        {
//            this.TestArgsMappedAuto("Method2", "--value1 --value2 invalid", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndPositionedAndFirstNegative()
//        {
//            this.TestArgsMappedAuto("Method2", "-1 1 1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method2AndFirstNegativeAndTheMiddleArgsWithPLUS()
//        {
//            this.TestArgsMappedAuto("Method2", "-1.10 +100 true", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method3AndSimpleTestIncludeDatetime()
//        {
//            this.TestArgsMappedAuto("Method3", "--value1 10.1223 --value2 10.2344 --date \"12-22/1879 12:10:10.111\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method3AndPositionedTestIncludeNegativeValueAndDatetime()
//        {
//            this.TestArgsMappedAuto("Method3", "-2000 -10.2222 \"01-01/2016 12:10:10.111\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method4And2ArgsAsFLAGAnd1ArgsBooleanWithNEGATIVEFormat()
//        {
//            this.TestArgsMappedAuto("Method4", "-ab --value-", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method4AndPositionedAllAsTrueValue()
//        {
//            this.TestArgsMappedAuto("Method4", "true - +", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5And2NamedWithDefaultsAnd1PositionedAndInvalid()
//        {
//            this.TestArgsMappedAuto("Method5", "--enum1 --enum2 invalid", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5And3NamedWithDefault()
//        {
//            this.TestArgsMappedAuto("Method5", "--enum1 --enum3 --enum4", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5AndInvalidEnumValueInMiddleAndShiftedTheRestByPosition()
//        {
//            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5AndInvalidEnumValueInMiddleButThePositionedFeaturedIsOFF()
//        {
//            this.TestArgsMappedAuto("Method5", "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", false, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5AndAllPositioned()
//        {
//            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 value1 1 2 3 4 5", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5AndAllPositioned2()
//        {
//            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 b 1 2 3 4 5 Value4", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method5AndAllPositionedExceptTheLast()
//        {
//            this.TestArgsMappedAuto("Method5", "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 --enum3 value1 1 2 3 4 5", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method6And3ListIntsDecimalsAndStrings()
//        {
//            this.TestArgsMappedAuto("Method6", "--lst 1 2 3 4 --lst2 10 10.1 10.2 10.333 --lst3 a b c \"1 2 3 4\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method6And3ListIntsDecimalsAndStringsPositioned()
//        {
//            this.TestArgsMappedAuto("Method6", "1 2 3 4 10 10.1 10.2 10.333 a b c \"\\\"quotes in content\\\"\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method7And2ListIntsAndChars()
//        {
//            this.TestArgsMappedAuto("Method7", "--lst 1 2 3 4 --lst2 a b c abc", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method7And2ListIntsAndCharsPositioned()
//        {
//            this.TestArgsMappedAuto("Method7", "-0.1 2 0.1000 4 a b c", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method8And5NamedNullableWithValues()
//        {
//            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2 --v3 10 --v4 1 0 1 0 --v5 a b c", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method8And5NamedNullableWithoutValuesSettingAsNull()
//        {
//            this.TestArgsMappedAuto("Method8", "--v1 --v2 --v3 --v4 --v5", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method8And2NamedAnd3Missing()
//        {
//            this.TestArgsMappedAuto("Method8", "--v1 1 --v2 2.10", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method8AndAllPositionedIncludeArrayOfBytesAndListOfChars()
//        {
//            this.TestArgsMappedAuto("Method8", "-10 -10.10 2000 0 1 0 1 0 1 a b c", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithValue()
//        {
//            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method9And2NamedUnsuporttedTypeAndDefaultValueWithoutValue()
//        {
//            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc --default-value", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method9And1NamedUnsuporttedTypeAnd1Missing()
//        {
//            this.TestArgsMappedAuto("Method9", "--unsuportted-type abc", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method9And2PositionedUnsuporttedTypeAndDefaultValueWithValue()
//        {
//            this.TestArgsMappedAuto("Method9", "abc 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10And2Named1ListOfStringAnd1Int()
//        {
//            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c --v2 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10And1Named1ListOfStringAnd1PositionedInt()
//        {
//            this.TestArgsMappedAuto("Method10", "--v1 \"abc\" a b c 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10And2Positioned1ListOfStringAnd1Int()
//        {
//            this.TestArgsMappedAuto("Method10", "\"abc\" a b c 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10And1Positioned1ListOfStringAnd1NamedInt()
//        {
//            this.TestArgsMappedAuto("Method10", "\"abc\" a b c --v2 20", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10AndSeveralValuesVariants()
//        {
//            this.TestArgsMappedAuto("Method10", "\\--value1 --=a --- --: -: -= -/ /= /- ", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10AndSeveralValuesVariants2()
//        {
//            this.TestArgsMappedAuto("Method10", "--v2 100000 lst1 lst2 --\"value1\":0 --value2=0 ", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10AndSeveralValuesVariants3()
//        {
//            this.TestArgsMappedAuto("Method10", "--2000 /0 --value=value--value=junior --value=\"value\" --v2 100000", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void Method10AndSeveralValuesVariants4()
//        {
//            this.TestArgsMappedAuto("Method10", "-abc=+ -def= + -1=0 \"--bla\" -l:=+\\\"quote\\\" --value=\"--value\"", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void MethodInvalidShortName()
//        {
//            try
//            {
//                this.TestArgsMappedAuto("Method11", "1234", true, TestHelper.GetCurrentMethodName());
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(ex.Message == "The map 'v1' has 'ShortName' invalid in method or class 'Method11'");
//            }
//        }

//        [TestMethod]
//        public void MethodInvalidLongNameEmpty()
//        {
//            try
//            {
//                this.TestArgsMappedAuto("Method12", "1234", true, TestHelper.GetCurrentMethodName());
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(ex.Message == "The map 'v1' has 'LongName' invalid in method or class 'Method12'");
//            }
//        }

//        [TestMethod]
//        public void MethodInvalidShortNameEmpty()
//        {
//            try
//            {
//                this.TestArgsMappedAuto("Method13", "1234", true, TestHelper.GetCurrentMethodName());
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(ex.Message == "The map 'v1' has 'ShortName' invalid in method or class 'Method13'");
//            }
//        }

//        [TestMethod]
//        public void MethodDuplicateShortName()
//        {
//            try
//            {
//                this.TestArgsMappedAuto("Method14", "1234", true, TestHelper.GetCurrentMethodName());
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(ex.Message == "The map 'v1' has the same 'ShortName' on the map 'v2' in method or class 'Method14'");
//            }
//        }

//        [TestMethod]
//        public void MethodDuplicateLongName()
//        {
//            try
//            {
//                this.TestArgsMappedAuto("Method15", "1234", true, TestHelper.GetCurrentMethodName());
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(ex.Message == "The map 'v1' has the same 'LogName' on the map 'v2' in method or class 'Method15'");
//            }
//        }

//        [TestMethod]
//        public void MethodLongNameWith1Char1()
//        {
//            this.TestArgsMappedAuto("Method16", "--v value1 -A value2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void MethodLongNameWith1Char2()
//        {
//            this.TestArgsMappedAuto("Method16", "-v value1 --value-UPPER value2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void MethodOrdered()
//        {
//            this.TestArgsMappedAuto("Method17", "--value-order-three value1 --value-order-two value2 --value-order-one value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void MethodOrdered2()
//        {
//            this.TestArgsMappedAuto("Method17", "value1 value2 value3", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void InstanceOrdered()
//        {
//            this.TestArgsMappedAuto(typeof(Class1), "--prop1 value1 --prop2 value2", true, false, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void InstanceOrdered2()
//        {
//            this.TestArgsMappedAuto(typeof(Class1), "value2 value1", true, false, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void InstanceAllProperties()
//        {
//            this.TestArgsMappedAuto(typeof(Class1), "--prop1 value1 --prop2 value2 --p value3 --prop4 value4 --prop5 value5", true, false, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void InstanceAllPropertiesOnlyWithAttributes()
//        {
//            this.TestArgsMappedAuto(typeof(Class1), "--prop1 value1 --prop2 value2 --p value3 --prop4 value4 --prop5 value5", true, true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutIgnoredsAndOnlyMethodsWithAttributes()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: true, usePrefixInAllMethods: false, prefix: "custom-prefix");
//            this.TestActionMapped(actionMaps, "clean -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutIgnoredsAndPrefixedWithClassName()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: null);
//            this.TestActionMapped(actionMaps, "git-clean", true, TestHelper.GetCurrentMethodName());
//        }
        
//        [TestMethod]
//        public void CallWithoutIgnoredsAndCustomPrefix()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: "custom-prefix");
//            this.TestActionMapped(actionMaps, "custom-prefix-main", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionMainAndEmptyArguments()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "main", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallEmptyArguments()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAnd1NamedArgument()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAnd1NamedArgumentAndMultiActionDisabled()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", false, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAndAllArgsPositioned()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"\--args a b c d", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAndAllArgsPositionedAnd1ActionScaped()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"\main a b c d", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallTwoActionsMainInSameCommand()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "main --args a b c d main 123", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallMainActionAnd1ActionScaped()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"main --args a b c d \\main 123", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAnd2ArgNamed()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "-a 1 -b 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionAnd2ArgsPositioned()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionMainThatIsIgnoredAction()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"main -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallTwoCallAndFirstIsArgsOfDefaulActionAndRestAreArgsOfMainAction()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"\main main 1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallMethodDefaultWithPositionedArgs()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"method-default -a value1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionsAnd1NamedArg()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "-a value1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallWithoutActionsAnd1PositionedArg()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "value1", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionCleanMethodWith2ArgsPositioned()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "clean 1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        public void CallActionCleanScapedThenAllArgsPositioned()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"\clean 1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionAddWithAllArgsPositionedButThisActionHasPosicionalDisable()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, "add abc def", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionAddWithNamedArgsAndCallActionClearInMiddle()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"add -a \add -b clean clean 1 2", true, TestHelper.GetCurrentMethodName());
//        }

//        [TestMethod]
//        public void CallActionCommit()
//        {
//            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
//            this.TestActionMapped(actionMaps, @"commit a b", true, TestHelper.GetCurrentMethodName());
//        }

//        [Flags]
//        public enum EnumTest1
//        {
//            Enum1Value1 = 1,
//            Enum1Value2 = 2,
//            Enum1Value3 = 4,
//            Enum1Value4 = 8
//        }

//        [Flags]
//        public enum EnumTest2
//        {
//            Enum2Value1 = 1,
//            Enum2Value2 = 2,
//            Enum2Value3 = 4,
//            Enum2Value4 = 8
//        }

//        public enum EnumTestNoFlag
//        {
//            Value1 = 'a',
//            Value2 = 'b',
//            Value3 = 'c',
//            Value4 = 'd',
//        }

//        public enum EnumTestNoFlag2
//        {
//            Value1,
//            Value2,
//            Value3,
//            Value4,
//        }

//        public class Class1
//        {
//            [Argument(Position = 1)]
//            public string Prop1 { get; set; }

//            [Argument(Position = 0)]
//            public string Prop2 { get; set; }

//            [Argument(DefaultValue = "default", IsRequired = true, Help = "Help", ShowHelpComplement = true, ShortName = 'p', LongName = "p")]
//            public string Prop3 { get; set; }

//            public string Prop4 { get; set; }

//            [Argument]
//            public string Prop5 { get; set; }
//        }

//        public class Git
//        {
//            [Argument(Position = 0)]
//            public string Arg0 { get; set; }

//            public string a { get; set; }
//            public string b { get; set; }
	
//            public string Main()
//            {
//                return "Main()";
//            }
	
//            public string Main(string[] args)
//            {
//                return "Main(string[] args)";
//            }

//            [Action(Ignore = true)]
//            public string Main(string a, string b)
//            {
//                return "Main(string a, string b)";
//            }
	
//            [Action(IsDefault = true)]
//            public string MethodDefault(string a = null, int? b = null)
//            {
//                return "MethodDefault(string a = null, int? b = null)";
//            }
	
//            [Action(IsDefault = true)]
//            public string MethodDefault(string a = null)
//            {
//                return "MethodDefault(string a = null)";
//            }
	
//            public string Clean(string a, string b)
//            {
//                return "Clean(string a, string b)";
//            }

//            [Action(EnablePositionalArgs=false)]
//            public string Add(string a, string b)
//            {
//                return "Add(string a, string b)";
//            }

//            [Action(UsePrefix=false, Name="commit")]
//            public string CommitAllFiles(string a, string b)
//            {
//                return "CommitAllFiles(string a, string b)";
//            }
//        }

//        public void Method1(string a, string b, string c) { }
//        public void Method2(decimal value1, int value2, bool value3) { }
//        public void Method3(double value1, float value2, DateTime date) { }
//        public void Method4(bool a, bool b, bool value) { }
//        public void Method5(EnumTest1? enum1, EnumTest2 enum2, EnumTestNoFlag enum3, int[] lst, EnumTestNoFlag2 enum4) { }
//        public void Method6(List<double> lst, List<decimal> lst2, List<string> lst3) { }
//        public void Method7(List<float> lst, List<char> lst2) { }

//        public void Method8(int? v1, decimal? v2, long? v3, byte?[] v4, List<char?> v5) { }
//        public void Method9(Class1 unsuporttedType, int defaultValue = 10) { }
//        public void Method10(string[] v1, int v2) { }

//        // MethodInvalidName
//        public void Method11(
//            [Argument(LongName = "value", ShortName = '2', ShowHelpComplement = true, Help = "My help")]
//            string v1, int v2) { }

//        // MethodInvalidName empty longName
//        public void Method12(
//            [Argument(LongName = "", ShortName = 'a', ShowHelpComplement = true, Help = "My help")]
//            string v1, int v2) { }

//        // MethodInvalidName empty shortName
//        public void Method13(
//            [Argument(LongName = "a", ShortName = ' ', ShowHelpComplement = true, Help = "My help")]
//            string v1, int v2) { }

//        //MethodDuplicateName
//        public void Method14(
//            [Argument(LongName = "value", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
//            string v1,
//            [Argument(LongName = "value2", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
//            int v2) { }

//        //MethodDuplicateName
//        public void Method15(
//            [Argument(LongName = "value", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
//            string v1,
//            [Argument(LongName = "value", ShortName = 'a', ShowHelpComplement = true, Help = "My help")]
//            int v2) { }

//        //MethodLongNameWith1Char
//        public void Method16(
//            [Argument(LongName = "v", ShortName = 'v', ShowHelpComplement = true, Help = "My help")]
//            string value,
//            [Argument(LongName = "value-UPPER", ShortName = 'A', ShowHelpComplement = true, Help = "My help")]
//            bool flag) { }

//        //MethodOrdered
//        public void Method17(
//            string valueOrderThree,
//            [Argument(Position = 0)]
//            string valueOrderOne,
//            [Argument(Position = 1)]
//            string valueOrderTWO) { }
//    }
//}
