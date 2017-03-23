using System.IO;
using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Reflection;
using SysCommand.Compatibility;
using Newtonsoft.Json.Serialization;

namespace SysCommand.Tests.UnitTests.Common
{
    public static class TestFileHelper
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();

        private static string GetUniversalFileName(string fileName)
        {
            // fix to unix
            return fileName.Replace("\\", "/").Replace("//", "/");
        }

        public static bool FileExists(string fileName)
        {
            fileName = GetUniversalFileName(fileName);
            return File.Exists(fileName);
        }
        
        public static string GetContentFromFile(string fileName)
        {
            fileName = GetUniversalFileName(fileName);
            if (!File.Exists(fileName))
                return null;

            return File.ReadAllText(fileName);
        }

        public static void RemoveFile(string fileName)
        {
            fileName = GetUniversalFileName(fileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        public static void SaveContentToFile(string content, string fileName)
        {
            CreateFolderIfNeeded(fileName);
            WriteAllText(fileName, content);
        }

        /// <summary>
        /// Create the folder if not existing for a full file name
        /// </summary>
        /// <param name="filename">full path of the file</param>
        public static void CreateFolderIfNeeded(string filename)
        {
            filename = GetUniversalFileName(filename);
            string folder = System.IO.Path.GetDirectoryName(filename);
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
        }

        public static void WriteAllText(string filename, string content)
        {
            filename = GetUniversalFileName(filename);
            File.WriteAllText(filename, content);
        }

        #region Json

        public static string GetContentJsonFromObject(object obj, JsonSerializerSettings config = null)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            else
            {
                if (config == null)
                {
                    config = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        SerializationBinder = binder,
                        Formatting = Formatting.Indented
                    };
                }

                return JsonConvert.SerializeObject(obj, config.Formatting, config);
            }
        }

        public static T GetObjectFromContentJson<T>(string contentJson, JsonSerializerSettings config = null)
        {
            if (config == null)
            {
                config = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = binder,
                };
            }

            return JsonConvert.DeserializeObject<T>(contentJson, config);
        }

        public static TFile GetObjectFromFileJson<TFile>(string fileName, JsonSerializerSettings config = null)
        {
            fileName = GetUniversalFileName(fileName);
            var objFile = default(TFile);

            if (File.Exists(fileName))
                objFile = GetObjectFromContentJson<TFile>(GetContentFromFile(fileName), config);

            return objFile;
        }

        public static void SaveObjectToFileJson(object obj, string fileName, JsonSerializerSettings config = null)
        {
            SaveContentToFile(GetContentJsonFromObject(obj, config), fileName);
        }

        #endregion

        internal class TypeNameSerializationBinder : ISerializationBinder
        {
            public TypeNameSerializationBinder()
            {
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = serializedType.Assembly().FullName;
                typeName = serializedType.FullName;
            }

            public Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType(typeName + ", " + assemblyName, true);
            }
        }
    }
}