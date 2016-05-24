using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;
using Newtonsoft.Json;

namespace SysCommand
{
    public class Request
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        public string CommandLine { get; private set; }
        public string[] Arguments { get; private set; }
        public string GetRaw { get; private set; }
        public string PostRaw { get; private set; }
        public Dictionary<string, string> Get { get; private set; }
        public List<SubCommand> SubCommands { get; private set; }
        
        public Request(string[] args)
        {
            this.Arguments = args;
            this.GetRaw = string.Join(" ", args);
            this.Get = AppHelpers.ArgsToDictionary(args);
            this.SubCommands = GetSubCommands(args);
            if (Console.IsInputRedirected && Console.In != null)
                this.PostRaw = Console.In.ReadToEnd();
        }

        public T Post<T>()
        {
            var objFile = default(T);

            if (string.IsNullOrWhiteSpace(this.PostRaw))
            {
                objFile = JsonConvert.DeserializeObject<T>(this.PostRaw, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });
            }

            return objFile;
        }

        public static List<SubCommand> GetSubCommands(string[] args)
        {
            var subCommands = new List<SubCommand>();

            if (App.Current.ActionCharPrefix != null)
            {
                var currentSubCommand = default(SubCommand);
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    {
                        if (arg[0].In(App.Current.ActionCharPrefix.Value))
                        {
                            currentSubCommand = new SubCommand();
                            currentSubCommand.Name = arg.Substring(1);
                            currentSubCommand.Position = i;
                            subCommands.Add(currentSubCommand);
                        }
                        else
                        {
                            if (currentSubCommand != null)
                                currentSubCommand.Arguments.Add(arg);
                        }
                    }
                }
            }
            else
            {
                var currentSubCommand = new SubCommand();
                subCommands.Add(currentSubCommand);

                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (i == 0 && !arg[0].In('-', '/', ':', '='))
                    {
                        currentSubCommand.Name = arg;
                        currentSubCommand.Position = i;
                    }
                    else
                    {
                        if (currentSubCommand != null)
                            currentSubCommand.Arguments.Add(arg);
                    }
                }
            }

            return subCommands;
        }
    }
}
