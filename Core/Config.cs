using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace SysCommand
{
    public abstract class Config
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        private string fileName;

        public Config(string fileName)
        {
            this.fileName = fileName;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });

            File.WriteAllText(this.fileName, json);
        }


        public static TConfig Get<TConfig>(string fileName) where TConfig : Config
        {
            var config = default(TConfig);

            if (File.Exists(fileName))
            {
                config = JsonConvert.DeserializeObject<TConfig>(File.ReadAllText(fileName), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });

                config.fileName = fileName;
            }

            if (config == null)
            {
                var ctors = typeof(TConfig).GetConstructors();
                if (ctors.Any(f => f.GetParameters().Length == 1 && f.GetParameters()[0].ParameterType == typeof(string)))
                    config = (TConfig)Activator.CreateInstance(typeof(TConfig), fileName);
                else
                    throw new Exception("The '" + typeof(TConfig).Name + " must contain an empty constructor or a parameter string to determine the file name.");

                if (config.fileName != fileName)
                    throw new Exception("The name of the requested file is not equal to the specified file name in the instance of type '" + typeof(TConfig).Name + "'");
            }

            return config;
        }
    }
}
