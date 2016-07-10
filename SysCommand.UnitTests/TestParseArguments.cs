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
        }

        private string GetTestFileName(string methodName, params object[] fileNames)
        {
            var fileNameFormat = @"Tests\{0}-{1}\{2}.json";
            var fileName = string.Format(fileNameFormat, this.GetType().Name, methodName, string.Join("", fileNames));
            return Path.Combine(@"..\..\", fileName);
        }

        private void SaveFileIfNotExists<T>(T obj, string fileName)
        {
            if (FileHelper.GetObjectFromFileJson<T>(fileName, jsonSerializeConfig) == null)
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

        private void TestArgsMappedAuto(string methodName, string input, string fileName, string testDetail = null)
        {
            var parser = new ArgumentsParser();
            var method = GetMethod(methodName);
            var args = AppHelpers.CommandLineToArgs(input);
            var maps = ArgumentsParser.GetArgumentMaps(method);
            var argsMappeds = parser.Parser(args, maps.ToArray());
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
            var currentMethodName = GetCurrentMethod();
            var methodName = "";
            
            // method 1
            methodName = "Method1";
            this.TestArgsMappedAuto(methodName, "method1 --a value1 --b value2 -c value3", this.GetTestFileName(currentMethodName, methodName, "-", i++));
            this.TestArgsMappedAuto(methodName, "method1 -a value1 -b -c value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method1 -abc value1", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "-b value1 -c value3 value2", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With inverted order, ignored wrong order");
            this.TestArgsMappedAuto(methodName, "-a value1 value2 value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -a");
            this.TestArgsMappedAuto(methodName, "-a value1 value2 -c value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -a and -c");
            this.TestArgsMappedAuto(methodName, "-a value1 -b value2 value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -a and -b");
            this.TestArgsMappedAuto(methodName, "value1 -b value2 value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -b");
            this.TestArgsMappedAuto(methodName, "value1 -b value2 -c value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -b and -c");
            this.TestArgsMappedAuto(methodName, "value1 value2 -c value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "With -c");
            this.TestArgsMappedAuto(methodName, "value1 value2 value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "None args");
            this.TestArgsMappedAuto(methodName, "value1 value2", this.GetTestFileName(currentMethodName, methodName, "-", i++) , "Two args only");

            //method2
            methodName = "Method2";
            this.TestArgsMappedAuto(methodName, "method2 --value1 10.12 --value2 10 --value3 true", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method2 --value1 10.12 --value2 10 --value3 0", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method2 --value1 10.12 --value2 10 --value3 +", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method2 --value1 10.12 --value2 10 --value3", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method2 --value1 --value2", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            
            //method3
            methodName = "Method3";
            this.TestArgsMappedAuto(methodName, "method3 --value1 10.1223 --value2 10.2344 --date \"12-22/1879 12:10:10.111\"", this.GetTestFileName(currentMethodName, methodName, "-", i++) );

            //method4
            methodName = "Method4";
            this.TestArgsMappedAuto(methodName, "Method4 -ab --value-", this.GetTestFileName(currentMethodName, methodName, "-", i++) );

            //method5
            methodName = "Method5";
            this.TestArgsMappedAuto(methodName, "method5 --enum1 --enum2", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
            this.TestArgsMappedAuto(methodName, "method5 --enum1 Enum1Value1 2 enum1value3 8 --enum2 Enum2Value1 Enum2value2 Enum2value2Error --enum3 value1 Value2 Value3 --lst 1 2 3 4 5 a 8", this.GetTestFileName(currentMethodName, methodName, "-", i++) );

            //method6
            methodName = "Method6";
            this.TestArgsMappedAuto(methodName, "method6 --lst 1 2 3 4 --lst2 10 10.1 10.2 10.333 --lst3 a b c \"1 2 3 4\"", this.GetTestFileName(currentMethodName, methodName, "-", i++) );

            //method7
            methodName = "Method7";
            this.TestArgsMappedAuto(methodName, "--lst 1 2 3 4 --lst2 a b c abc", this.GetTestFileName(currentMethodName, methodName, "-", i++) );
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

        public void Method1(string a, string b, string c) { }
        public void Method2(decimal value1, int value2, bool value3) { }
        public void Method3(double value1, float value2, DateTime date) { }
        public void Method4(bool a, bool b, bool value) { }
        public void Method5(EnumTest1 enum1, EnumTest2 enum2, EnumTestNoFlag enum3, int[] lst) { }
        public void Method6(List<double> lst, List<decimal> lst2, List<string> lst3) { }
        public void Method7(List<float> lst, List<char> lst2) { }
    }
}
