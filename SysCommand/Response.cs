using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysCommand
{
    public class Response
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        public int Code { get; set; }

        public void Write(string value, params object[] args)
        {
            Console.Write(value, args);
        }

        public void WriteLine(string value, params object[] args)
        {
            Console.WriteLine(value, args);
        }

        public void Post(object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });

            Console.Write(json);
        }
    }
}
