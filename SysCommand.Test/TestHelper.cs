using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SysCommand.Test
{
    public static class TestHelper
    {
        public static void SetCultureInfoToInvariant()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public static JsonSerializerSettings GetJsonConfig()
        {
            var jsonSerializeConfig = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            jsonSerializeConfig.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            jsonSerializeConfig.TypeNameHandling = TypeNameHandling.None;
            return jsonSerializeConfig;
        }

        private static string Join(string separator, params string[] values)
        {
            return string.Join(separator, values);
        }

        private static string GetFileName(string typeName, string testContext, string fileNameSuffix, string fileName, string fileNameFinal)
        {
            testContext = testContext != null ? "-" + testContext : "";
            var fileNameFormat = @"Tests\{0}{1}\{2}\{3}.json{4}";
            var path = string.Format(fileNameFormat, typeName, testContext, fileNameSuffix, fileName, fileNameFinal);
            return Path.Combine(@"..\..\", path);
        }

        private static string GetValidTestFileName(string typeName, string testContext, string fileName)
        {
            return GetFileName(typeName, testContext, "valid", fileName, "");
        }

        private static string GetInvalidTestFileName(string typeName, string testContext, string fileName)
        {
            return GetFileName(typeName, testContext, "invalid", fileName, "");
        }

        private static string GetUncheckedTestFileName(string typeName, string testContext, string fileName)
        {
            return GetFileName(typeName, testContext, "unchecked", fileName, "");
        }

        private static void SaveUncheckedFileIfValidNotExists<T>(string typeName, T obj, string testContext, string fileName, JsonSerializerSettings config)
        {
            var validFileName = GetValidTestFileName(typeName, testContext, fileName);
            var uncheckedFileName = GetUncheckedTestFileName(typeName, testContext, fileName);
            if (!TestFileHelper.FileExists(validFileName))
                TestFileHelper.SaveObjectToFileJson(obj, uncheckedFileName, config);
        }

        private static void SaveInvalidFileIfValidExists<T>(string typeName, T obj, string testContext, string fileName, JsonSerializerSettings config)
        {
            var validFileName = GetValidTestFileName(typeName, testContext, fileName);
            var invalidFileName = GetInvalidTestFileName(typeName, testContext, fileName);
            if (TestFileHelper.FileExists(validFileName))
                TestFileHelper.SaveObjectToFileJson(obj, invalidFileName, config);
        }

        private static void RemoveInvalidFile(string typeName, string testContext, string fileName)
        {
            var invalidFileName = GetInvalidTestFileName(typeName, testContext, fileName);
            if (TestFileHelper.FileExists(invalidFileName))
                TestFileHelper.RemoveFile(invalidFileName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public static bool CompareObjects<TType>(object objectTest, string testContext, string testMethodName, JsonSerializerSettings config = null)
        {
            config = config ?? GetJsonConfig();

            var typeName = typeof(TType).Name;
            //// add if not exists, and the first add must be correct
            SaveUncheckedFileIfValidNotExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);

            var outputTest = TestFileHelper.GetContentJsonFromObject(objectTest, config);
            var outputCorrect = TestFileHelper.GetContentFromFile(GetValidTestFileName(typeName, testContext, testMethodName));
            var test = outputTest == outputCorrect;

            if (!test)
                SaveInvalidFileIfValidExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);
           
            return outputTest == outputCorrect;
        }
    }
}
