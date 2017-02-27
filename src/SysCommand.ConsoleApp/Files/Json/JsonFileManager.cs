using Newtonsoft.Json;
using SysCommand.ConsoleApp.Helpers;
using SysCommand.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if NETSTANDARD1_6
using SysCommand.Reflection;
#endif

namespace SysCommand.ConsoleApp.Files
{
    public class JsonFileManager
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        private Dictionary<string, object> objectsFiles = new Dictionary<string, object>();
        private string defaultFolder;

        public string DefaultFilePrefix { get; set; }
        public string DefaultFileExtension { get; set; }
        public bool UseTypeFullName { get; set; }
        public bool SaveInRootFolderWhenIsDebug { get; set; }

        public string DefaultFolder
        {
            get
            {
                return this.defaultFolder;
            }
            set
            {
                this.defaultFolder = value;
                if (DebugHelper.IsDebug && this.SaveInRootFolderWhenIsDebug)
                    this.defaultFolder = Path.Combine(@"..\..\", this.defaultFolder);
            }
        }

        public JsonFileManager()
        {
            this.SaveInRootFolderWhenIsDebug = true;
            this.DefaultFolder = ".app";
            this.DefaultFilePrefix = "";
            this.DefaultFileExtension = ".json";
        }

        public void Save<T>(T obj)
        {
            var fileName = this.GetObjectFileName(typeof(T));
            this.SaveInternal(obj, this.GetFilePath(fileName));
        }

        public void Save(object obj, string fileName)
        {
            this.SaveInternal(obj, this.GetFilePath(fileName));
        }

        private void SaveInternal(object obj, string fileName)
        {
            if (obj != null)
            {
                SaveToFileJson(obj, fileName);
                this.objectsFiles[fileName] = obj;
            }
        }

        public void Remove<T>()
        {
            var fileName = this.GetObjectFileName(typeof(T));
            this.RemoveInternal(this.GetFilePath(fileName));
        }

        public void Remove(string fileName)
        {
            this.RemoveInternal(this.GetFilePath(fileName));
        }

        private void RemoveInternal(string fileName)
        {
            FileHelper.RemoveFile(fileName);
            if (this.objectsFiles.ContainsKey(fileName))
                this.objectsFiles.Remove(fileName);
        }

        public T Get<T>(string fileName = null, bool refresh = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(T));

            return GetOrCreateInternal<T>(this.GetFilePath(fileName), true, refresh);
        }

        public T GetOrCreate<T>(string fileName = null, bool refresh = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(T));

            return GetOrCreateInternal<T>(this.GetFilePath(fileName), false, refresh);
        }

        private T GetOrCreateInternal<T>(string fileName = null, bool onlyGet = false, bool refresh = false)
        {
            if (this.objectsFiles.ContainsKey(fileName) && !refresh)
                return this.objectsFiles[fileName] == null ? default(T) : (T)this.objectsFiles[fileName];

            var objFile = GetFromFileJson<T>(fileName);

            if (objFile == null && !onlyGet)
                objFile = Activator.CreateInstance<T>();

            this.objectsFiles[fileName] = objFile;

            return objFile;
        }

        public string GetObjectFileName(Type type)
        {
            string fileName;

            var attr = type.GetCustomAttribute<ObjectFileAttribute>(true);
            if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
            {
                fileName = attr.FileName;
            }
            else
            {
                fileName = ReflectionHelper.CSharpName(type, this.UseTypeFullName).Replace("<", "[").Replace(">", "]").Replace(@"\", "");
                fileName = this.DefaultFilePrefix + StringHelper.ToLowerSeparate(fileName, '.') + this.DefaultFileExtension;
            }

            return fileName;
        }

        public string GetFilePath(string fileName)
        {
            return Path.Combine(this.DefaultFolder, fileName);
        }

#region Json

        public static string GetContentJsonFromObject(object obj, JsonSerializerSettings config = null)
        {
            if (config == null)
            {
                config = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };

#if NETSTANDARD1_6
                config.SerializationBinder = binder;
#else
                config.Binder = binder;
#endif
            }

            return JsonConvert.SerializeObject(obj, config.Formatting, config);
        }

        public static T GetFromContentJson<T>(string contentJson, JsonSerializerSettings config = null)
        {
            if (config == null)
            {
                config = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

#if NETSTANDARD1_6
                config.SerializationBinder = binder;
#else
                config.Binder = binder;
#endif
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
