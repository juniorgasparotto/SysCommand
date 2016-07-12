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
        private bool GenerateFilesResultsToValidate = false;

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

        private string GetTestFileName(string methodName, params object[] fileNames)
        {
            var fileNameFormat = @"Tests\{0}-{1}\{2}{3}.json";
            var valid = this.GenerateFilesResultsToValidate ? "" : "valid-";
            var fileName = string.Format(fileNameFormat, this.GetType().Name, methodName, valid, string.Join("", fileNames));
            
            return Path.Combine(@"..\..\", fileName);
        }

        private void SaveFileIfNotExists<T>(T obj, string fileName)
        {
            if (this.GenerateFilesResultsToValidate && FileHelper.GetObjectFromFileJson<T>(fileName, jsonSerializeConfig) == null)
                FileHelper.SaveObjectToFileJson(obj, fileName, jsonSerializeConfig);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        [TestMethod]
        public void TestSingle()
        {
            var parser = new ArgumentsParser();
            var args = AppHelpers.CommandLineToArgs("action --teste -xyz+ --bla -u false -i+ teste --\"abc\":0");
            var dic = parser.Parser(args);
            var output = ToString(dic);
            Assert.IsTrue(output == "[]=[action];[teste]=[];[x]=[true];[y]=[true];[z]=[true];[bla]=[];[u]=[false];[i]=[true];[]=[teste];[abc]=[0];");

            args = AppHelpers.CommandLineToArgs("action --teste+ -x- \"--bla\" /u false -iih=+ , -l:=+\\\"quote\\\" -1=0 2 3");
            dic = parser.Parser(args);
            output = ToString(dic);
            Assert.IsTrue(output == "[]=[action];[teste]=[true];[x]=[false];[bla]=[];[u]=[false];[i]=[+];[i]=[+];[h]=[+];[]=[,];[l]=[+\"quote\"];[1]=[0];[]=[2];[]=[3];");

            args = AppHelpers.CommandLineToArgs("--a value1 --b value2 -c value3");
            dic = parser.Parser(args);
            output = ToString(dic);
            Assert.IsTrue(output == "[a]=[value1];[b]=[value2];[c]=[value3];");
        }

        private void TestArgsMappedAuto(string methodName, string input, bool enablePositionedArgs, string fileName, string testDetail = null)
        {
            var parser = new ArgumentsParser();
            var method = GetMethod(methodName);
            var args = AppHelpers.CommandLineToArgs(input);
            var maps = ArgumentsParser.GetArgumentMaps(method);
            var argsMappeds = parser.Parser(args, enablePositionedArgs, maps.ToArray());
            var objectTest = new { input, maps, argsMappeds };

            // add if not exists, and the first add must be correct
            this.SaveFileIfNotExists<dynamic>(objectTest, fileName);

            var outputTest = FileHelper.GetContentJsonFromObject(objectTest, jsonSerializeConfig);
            var outputCorrect = FileHelper.GetContentFromFile(fileName);
            Assert.IsTrue(outputTest == outputCorrect, string.Format("Error is test file '{0}'. {1}", fileName, testDetail));
        }

        [TestMethod]
        public void TestArgsMapped()
        {
            var i = 0;
            var n = 0;
            var currentMethodName = GetCurrentMethod();
            var methodName = "";
            
            // method 1
            methodName = "Method1";
            this.TestArgsMappedAuto(methodName, "value-positioned --a value1 --b value2 -c value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-positioned-3-named"));            
            this.TestArgsMappedAuto(methodName, "-a value1 -b -c value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-named-1-without-value"));
            this.TestArgsMappedAuto(methodName, "-abc value1", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-args-in-1"));
            this.TestArgsMappedAuto(methodName, "-b value1 -c value3 value2", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-1-positioned-in-last"), "With inverted order, ignored wrong order");
            this.TestArgsMappedAuto(methodName, "-a value1 value2 value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-named-2-positioned"), "With -a");
            this.TestArgsMappedAuto(methodName, "-a value1 value2 -c value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-1-positioned-in-middle"), "With -a and -c");
            this.TestArgsMappedAuto(methodName, "-a value1 -b value2 value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-1-positioned-in-last2"), "With -a and -b");
            this.TestArgsMappedAuto(methodName, "value1 -b value2 value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-named-in-middle-and-2-positioned"), "With -b");
            this.TestArgsMappedAuto(methodName, "value1 -b value2 -c value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-named-in-first-and-2-named"), "With -b and -c");
            this.TestArgsMappedAuto(methodName, "value1 value2 -c value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-positioned-1-named-in-last"), "With -c");
            this.TestArgsMappedAuto(methodName, "value1 value2 value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned"), "None args");
            this.TestArgsMappedAuto(methodName, "value1 value2", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-positioned-without-1-arg"), "Two args only");
            this.TestArgsMappedAuto(methodName, "value1 \"\\\"quote in content\\\"\"", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-positioned-with-quote-in-content"));
            this.TestArgsMappedAuto(methodName, @"\--value1 \-value2 \/value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned-and-args-format-scapeds"));
            this.TestArgsMappedAuto(methodName, @"\\--value1 \\-value2 \\/value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned-and-scape-args-as-value"));
            this.TestArgsMappedAuto(methodName, @"- \- /", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned-and-args-format-as-value"), "Scape args as value");
            this.TestArgsMappedAuto(methodName, @"-a - -b \- \/", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-1-positioned-and-all-args-has-format-as-value"));
            this.TestArgsMappedAuto(methodName, @"--a \--value1 -b \/ -c --", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-named-with-scapes"));
            this.TestArgsMappedAuto(methodName, @"--a \ \ -c \", true, this.GetTestFileName(currentMethodName, methodName, "-", "backslash-as-values"));

            //method2
            methodName = "Method2";
            this.TestArgsMappedAuto(methodName, "--value1 10.12 --value2 10 --value3 true", true, this.GetTestFileName(currentMethodName, methodName, "-", "boolean-as-true"));
            this.TestArgsMappedAuto(methodName, "--value1 10.12 --value2 10 --value3 0", true, this.GetTestFileName(currentMethodName, methodName, "-", "boolean-as-0"));
            this.TestArgsMappedAuto(methodName, "--value1 10.12 --value2 10 --value3 +", true, this.GetTestFileName(currentMethodName, methodName, "-", "boolean-as-PLUS"));
            this.TestArgsMappedAuto(methodName, "--value1 10.12 --value2 10 --value3", true, this.GetTestFileName(currentMethodName, methodName, "-", "boolean-as-FLAG"));
            this.TestArgsMappedAuto(methodName, "--value1 -10.12 --value2 -10 --value3 -1", true, this.GetTestFileName(currentMethodName, methodName, "-", "decimal-and-int-as-negative-and-boolean-invalid"));
            this.TestArgsMappedAuto(methodName, "--value1 --value2 invalid", true, this.GetTestFileName(currentMethodName, methodName, "-", "last-arg-positioned-and-invalid-and-others-as-default"), "default type value and invalid value");
            this.TestArgsMappedAuto(methodName, "-1 1 1", true, this.GetTestFileName(currentMethodName, methodName, "-", "positioned-and-first-negative"));
            this.TestArgsMappedAuto(methodName, "-1.10 +100 true", true, this.GetTestFileName(currentMethodName, methodName, "-", "first-negative-and-the-middle-args-with-PLUS"));
            
            //method3
            methodName = "Method3";
            this.TestArgsMappedAuto(methodName, "--value1 10.1223 --value2 10.2344 --date \"12-22/1879 12:10:10.111\"", false, this.GetTestFileName(currentMethodName, methodName, "-", "simple-test-include-datetime"));
            this.TestArgsMappedAuto(methodName, "-2000 -10.2222 \"01-01/2016 12:10:10.111\"", true, this.GetTestFileName(currentMethodName, methodName, "-", "positioned-test-include-negative-value-and-datetime"));

            //method4
            methodName = "Method4";
            this.TestArgsMappedAuto(methodName, "-ab --value-", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-args-as-FLAG-and-1-args-boolean-with-NEGATIVE-format"));
            this.TestArgsMappedAuto(methodName, "true - +", true, this.GetTestFileName(currentMethodName, methodName, "-", "positioned-all-as-true-value"));

            //method5
            methodName = "Method5";
            this.TestArgsMappedAuto(methodName, "--enum1 --enum2 invalid", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-with-defaults-and-1-positioned-and-invalid"));
            this.TestArgsMappedAuto(methodName, "--enum1 --enum3 --enum4", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-named-with-default"));
            this.TestArgsMappedAuto(methodName, "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", true, this.GetTestFileName(currentMethodName, methodName, "-", "invalid-enum-value-in-middle-and-shifted-the-rest-by-position"), "Test with invalid enum value 'Enum2value2Error' that where the args are shifted, by position, and cause a strange result.");
            this.TestArgsMappedAuto(methodName, "--enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", false, this.GetTestFileName(currentMethodName, methodName, "-", "invalid-enum-value-in-middle-but-the-positioned-featured-is-OFF"), "The without position");

            this.TestArgsMappedAuto(methodName, "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 value1 1 2 3 4 5", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned"));
            this.TestArgsMappedAuto(methodName, "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 b 1 2 3 4 5 Value4", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned2"));
            this.TestArgsMappedAuto(methodName, "Enum1Value1 2 enum1value3 8 Enum2Value1 Enum2value2 --enum3 value1 1 2 3 4 5", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned-except-the-last"));

            //method6
            methodName = "Method6";
            this.TestArgsMappedAuto(methodName, "--lst 1 2 3 4 --lst2 10 10.1 10.2 10.333 --lst3 a b c \"1 2 3 4\"", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-list-ints-decimals-and-strings"));
            this.TestArgsMappedAuto(methodName, "1 2 3 4 10 10.1 10.2 10.333 a b c \"\\\"quotes in content\\\"\"", true, this.GetTestFileName(currentMethodName, methodName, "-", "3-list-ints-decimals-and-strings-positioned"));

            //method7
            methodName = "Method7";
            this.TestArgsMappedAuto(methodName, "--lst 1 2 3 4 --lst2 a b c abc", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-list-ints-and-chars"));
            this.TestArgsMappedAuto(methodName, "-0.1 2 0.1000 4 a b c", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-list-ints-and-chars-positioned"));

            //method8
            methodName = "Method8";
            this.TestArgsMappedAuto(methodName, "--v1 1 --v2 2 --v3 10 --v4 1 0 1 0 --v5 a b c", true, this.GetTestFileName(currentMethodName, methodName, "-", "5-named-nullable-with-values"));
            this.TestArgsMappedAuto(methodName, "--v1 --v2 --v3 --v4 --v5", true, this.GetTestFileName(currentMethodName, methodName, "-", "5-named-nullable-without-values-setting-as-null"));
            this.TestArgsMappedAuto(methodName, "--v1 1 --v2 2.10", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-and-3-missing"));
            this.TestArgsMappedAuto(methodName, "-10 -10.10 2000 0 1 0 1 0 1 a b c", true, this.GetTestFileName(currentMethodName, methodName, "-", "all-positioned-include-array-of-bytes-and-list-of-chars"));

            //method9
            //(Class1 unsuporttedType, int defaultValue = 10) { }
            methodName = "Method9";
            this.TestArgsMappedAuto(methodName, "--unsuporttedType abc --defaultValue 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-unsuportted-type-and-default-value-with-value"));
            this.TestArgsMappedAuto(methodName, "--unsuporttedType abc --defaultValue", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-unsuportted-type-and-default-value-without-value"));
            this.TestArgsMappedAuto(methodName, "--unsuporttedType abc", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-named-unsuportted-type-and-1-missing"));
            this.TestArgsMappedAuto(methodName, "abc 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-positioned-unsuportted-type-and-default-value-with-value"));

            methodName = "Method10";
            this.TestArgsMappedAuto(methodName, "--v1 \"abc\" a b c --v2 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-named-1-list-of-string-and-1-int"));
            this.TestArgsMappedAuto(methodName, "--v1 \"abc\" a b c 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-named-1-list-of-string-and-1-positioned-int"));
            this.TestArgsMappedAuto(methodName, "\"abc\" a b c 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "2-positioned-1-list-of-string-and-1-int"));
            this.TestArgsMappedAuto(methodName, "\"abc\" a b c --v2 20", true, this.GetTestFileName(currentMethodName, methodName, "-", "1-positioned-1-list-of-string-and-1-named-int"));
        }

        private string ToString(IEnumerable<ArgumentsParser.ArgumentSimple> items)
        {
            var output = "";
            foreach (var item in items)
            {
                output += "[" + item.Key + "]=[" + item.Value + "];";
            }
            return output;
        }

        private string ToString(IEnumerable<ArgumentsParser.ArgumentMapped> items)
        {
            var output = "";
            foreach (var item in items)
            {   
                var value = item.Value;
                if (value != null && ((item.Type.IsGenericType && item.Type.GetGenericTypeDefinition() == typeof(List<>)) || item.Type.IsArray))
                {

                    var enumerator = (value as System.Collections.IEnumerable).GetEnumerator();
                    var valueJoin = "";
                    while(enumerator.MoveNext())
                    {
                        valueJoin += valueJoin == "" ? enumerator.Current : "," + enumerator.Current;
                    }
                    value = valueJoin;
                }
                output += "[" + item.Key + "]=[" + value + "];";
            }
            return output;
        }

        private MethodInfo GetMethod(string name)
        {
            return this.GetType().GetMethod(name);
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
