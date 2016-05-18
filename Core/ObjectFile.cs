using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace SysCommand
{
    public class ObjectFile<TOFile>
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        public string FileName { get; private set; }
        public TOFile Object { get; private set; }

        private ObjectFile(string fileName)
        {
            this.FileName = fileName;
        }

        public void Save()
        {
            Save(this.Object, this.FileName);
        }

        public static void Save(TOFile obj, string fileName)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });

            AppHelpers.CreateFolderIfNeeded(fileName);
            File.WriteAllText(fileName, json);
        }

        public static ObjectFile<TOFile> GetOrCreate(string fileName, bool onlyGet = false)
        {
            var objFile = new ObjectFile<TOFile>(fileName);

            if (File.Exists(fileName))
            {
                var obj = JsonConvert.DeserializeObject<TOFile>(File.ReadAllText(fileName), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });
                objFile.Object = obj;
            }

            if (onlyGet)
                return objFile;

            if (objFile.Object == null)
                objFile.Object = Activator.CreateInstance<TOFile>();

            return objFile;
        }
    }
}
