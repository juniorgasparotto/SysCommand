using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Globalization;
using SysCommand.ConsoleApp.Helpers;
using System.Reflection;
using SysCommand.Compatibility;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests.UnitTests.Common
{
    public static class TestHelper
    {
        public static string FolderTests;
        private static IEnumerable<Type> startupTypes;

        public static IEnumerable<Type> StartupTypes
        {
            get
            {
                if (startupTypes == null)
                {
                    var assemblies = ReflectionCompatibility.GetAssemblies().Where(f => f.FullName.Contains("SysCommand")).ToList();
                    startupTypes = (from domainAssembly in assemblies
                                    from assemblyType in domainAssembly.GetTypes()
                                    where
                                           typeof(IStartup).IsAssignableFrom(assemblyType)
                                        && assemblyType.IsInterface() == false
                                        && assemblyType.IsAbstract() == false
                                    select assemblyType).ToList();
                }

                return startupTypes;
        }

        }

        public static void Setup()
        {
            SetCurrentDirectoryToRootProjectFolder();
            SetCultureInfoToInvariant();
            RunInitializeClasses();
        }

        private static void SetCurrentDirectoryToRootProjectFolder()
        {
#if NET452
            Directory.SetCurrentDirectory(Development.GetProjectDirectory());
#else //  when is NETCORE and use xunit in visual studio, the current directory is wrong when use Directory.GetCurrentDirectory()
            var baseDir =  Path.GetDirectoryName(typeof(TestHelper).GetTypeInfo().Assembly.Location);
            Directory.SetCurrentDirectory(Development.GetProjectDirectory(baseDir));
#endif
        }

        private static void RunInitializeClasses()
        {
            foreach(var t in StartupTypes)
            {
                var nt = (IStartup) Activator.CreateInstance(t);
                nt.Start();
            }
        }

        public static void SetCultureInfoToInvariant()
        {
#if NETCORE
            CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
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
            var folderTests = FolderTests ?? ".tests";
            var path = $@"{folderTests}\{typeName}{testContext}\{fileNameSuffix}\{fileName}.json{fileNameFinal}";
            return path;
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
            config.Converters.Add(new TestObjectJsonConverter());

            var typeName = typeof(TType).Name;
            SaveUncheckedFileIfValidNotExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);

            var outputTest = TestFileHelper.GetContentJsonFromObject(objectTest, config);
            var outputCorrect = TestFileHelper.GetContentFromFile(GetValidTestFileName(typeName, testContext, testMethodName));
            var test = CompareString(outputTest, outputCorrect);

            if (!test)
                SaveInvalidFileIfValidExists<dynamic>(typeName, objectTest, testContext, testMethodName, config);
            return test;
        }

        public static bool CompareString(string a, string b) {
            string compareA = a.Replace("\r\n", "\n");
            string compareB = b.Replace("\r\n", "\n");
            return compareA == compareB;
        }
    }
}
