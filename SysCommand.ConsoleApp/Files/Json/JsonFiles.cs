using Newtonsoft.Json;
using SysCommand.ConsoleApp.Helpers;
using SysCommand.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SysCommand.ConsoleApp.Files
{
    public class JsonFiles
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        private Dictionary<string, object> objectsFiles = new Dictionary<string, object>();

        public string DefaultRootFolder { get; set; }
        public string DefaultFilePrefix { get; set; }
        public string DefaultFileExtension { get; set; }
        public bool UseTypeFullName { get; set; }
        public bool SaveInRootFolderWhenIsDebug { get; set; }

        public JsonFiles()
        {
            this.SaveInRootFolderWhenIsDebug = true;
            this.DefaultRootFolder = ".app";
            this.DefaultFilePrefix = "";
            this.DefaultFileExtension = ".object";
        }

        public void Save<T>(T obj, string fileName = null)
        {
            fileName = GetFileName<T>(fileName);

            if (obj != null)
            {
                SaveToFileJson(obj, fileName);
                this.objectsFiles[fileName] = obj;
            }
        }

        public void Remove<T>(string fileName = null)
        {
            fileName = GetFileName<T>(fileName);

            FileHelper.RemoveFile(fileName);
            if (this.objectsFiles.ContainsKey(fileName))
                this.objectsFiles.Remove(fileName);
        }

        public T Get<T>(string fileName = null, bool refresh = false)
        {
            return GetOrCreate<T>(fileName, true, refresh);
        }

        public T GetOrCreate<T>(string fileName = null, bool onlyGet = false, bool refresh = false)
        {
            fileName = GetFileName<T>(fileName);

            if (this.objectsFiles.ContainsKey(fileName) && !refresh)
                return this.objectsFiles[fileName] == null ? default(T) : (T)this.objectsFiles[fileName];

            var objFile = GetFromFileJson<T>(fileName);

            if (objFile == null && !onlyGet)
                objFile = Activator.CreateInstance<T>();

            this.objectsFiles[fileName] = objFile;

            return objFile;
        }

        public string GetObjectFileName(Type type, string fileName = null, bool useTypeFullName = false)
        {
            string folder = null;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var attr = type.GetCustomAttributes(typeof(ObjectFileAttribute), true).FirstOrDefault() as ObjectFileAttribute;
                if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
                {
                    fileName = attr.FileName;
                }
                else
                {
                    useTypeFullName = !useTypeFullName ? this.UseTypeFullName : useTypeFullName;
                    fileName = ReflectionHelper.CSharpName(type, useTypeFullName).Replace("<", "[").Replace(">", "]").Replace(@"\", "");
                    fileName = this.DefaultFilePrefix + StringHelper.ToLowerSeparate(fileName, '.') + this.DefaultFileExtension;
                }

                folder = this.DefaultRootFolder;
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Folder))
                    folder = attr.Folder;
            }

            if (string.IsNullOrWhiteSpace(folder))
                return this.GetPathFromRoot(fileName);
            else
                return this.GetPathFromRoot(folder, fileName);
        }

        private string GetFileName<T>(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return this.GetObjectFileName(typeof(T), fileName);
            else
                return this.GetPathFromRoot(this.DefaultRootFolder, fileName);
        }

        public string GetPathFromRoot(params string[] paths)
        {
            if (DebugHelper.IsDebug && this.SaveInRootFolderWhenIsDebug)
            {
                var paths2 = paths.ToList();
                paths2.Insert(0, @"..\..\");
                return Path.Combine(paths2.ToArray());
            }

            return Path.Combine(paths);
        }

        #region Json

        public static string GetContentJsonFromObject(object obj, JsonSerializerSettings config = null)
        {
            if (config == null)
            {
                config = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder,
                    Formatting = Formatting.Indented
                };
            }

            return JsonConvert.SerializeObject(obj, config.Formatting, config);
        }

        public static T GetFromContentJson<T>(string contentJson, JsonSerializerSettings config = null)
        {
            if (config == null)
            {
                config = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder,
                };
            }

            return JsonConvert.DeserializeObject<T>(contentJson, config);
        }

        public static T GetFromFileJson<T>(string fileName, JsonSerializerSettings config = null)
        {
            var objFile = default(T);

            if (File.Exists(fileName))
                objFile = GetFromContentJson<T>(FileHelper.GetContentFromFile(fileName), config);

            return objFile;
        }

        public static void SaveToFileJson(object obj, string fileName, JsonSerializerSettings config = null)
        {
            FileHelper.SaveContentToFile(GetContentJsonFromObject(obj, config), fileName);
        }

        #endregion

    }
}
