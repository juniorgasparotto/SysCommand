using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;

namespace SysCommand.TestUtils
{
    public static class TestHelper
    {
        public static void SetCultureInfoToInvariant()
        {
#if NETSTANDARD1_6

#else
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
#endif
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

#if NETSTANDARD1_6
            var mainFolder = "tests";
#else
            var mainFolder = ".tests";
#endif

            var fileNameFormat = mainFolder + @"\{0}{1}\{2}\{3}.json{4}";
            var path = string.Format(fileNameFormat, typeName, testContext, fileNameSuffix, fileName, fileNameFinal);

#if NETSTANDARD1_6
            return path;
#else
            return Path.Combine(@"..\..\", path);
#endif
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
        public static string GetCurrentMethodName(
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
        {
            return memberName;
        }

        public static bool CompareObjects<TType>(object objectTest, string testContext, string testMethodName, JsonSerializerSettings config = null)
        {
            config = config ?? GetJsonConfig();

            var typeName = typeof(TType).Name;
            SaveUncheckedFileIfValidNotExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);

            var outputTest = TestFileHelper.GetContentJsonFromObject(objectTest, config);
            var outputCorrect = TestFileHelper.GetContentFromFile(GetValidTestFileName(typeName, testContext, testMethodName));
            var test = outputTest == outputCorrect;

            if (!test)
                SaveInvalidFileIfValidExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);
            
            return outputTest == outputCorrect;
        }

        //public static bool CompareObjectsString<TType>(string objectTest, string testContext, string testMethodName, JsonSerializerSettings config = null)
        //{
        //    config = config ?? GetJsonConfig();

        //    var typeName = typeof(TType).Name;
        //    SaveUncheckedFileIfValidNotExists(typeName, objectTest, testContext, testMethodName, config);

        //    var outputTest = objectTest;
        //    var outputCorrect = TestFileHelper.GetContentFromFile(GetValidTestFileName(typeName, testContext, testMethodName));
        //    var test = outputTest == outputCorrect;

        //    if (!test)
        //        SaveInvalidFileIfValidExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);

        //    return outputTest == outputCorrect;
        //}
    }
}
