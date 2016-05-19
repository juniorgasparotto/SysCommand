using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace SysCommand
{
    public abstract class ObjectFile
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();

        public void Save(string fileName)
        {
            Save(this, fileName);
        }

        public static void Save<TOFile>(TOFile obj, string fileName)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });

            AppHelpers.CreateFolderIfNeeded(fileName);
            File.WriteAllText(fileName, json);
        }

        public static TOFile Get<TOFile>(string fileName)
        {
            var objFile = default(TOFile);

            if (File.Exists(fileName))
            {
                objFile = JsonConvert.DeserializeObject<TOFile>(File.ReadAllText(fileName), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });
            }

            return objFile;
        }

        public static void Remove(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
