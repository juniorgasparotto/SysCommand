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
        public List<RequestAction> Actions { get; private set; }
        
        public Request(string[] args)
        {
            this.Arguments = args;
            this.GetRaw = string.Join(" ", args);
            this.Get = AppHelpers.ArgsToDictionary(args);
            this.Actions = GetActions(args);
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

        public static List<RequestAction> GetActions(string[] args)
        {
            var actions = new List<RequestAction>();

            if (App.Current.ActionCharPrefix != null)
            {
                var currentAction = default(RequestAction);
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    {
                        if (arg[0].In(App.Current.ActionCharPrefix.Value))
                        {
                            currentAction = new RequestAction();
                            currentAction.Name = arg.Substring(1);
                            currentAction.Position = i;
                            actions.Add(currentAction);
                        }
                        else
                        {
                            if (currentAction != null)
                            {
                                if (arg.Length > 2)
                                    arg = (arg[0] == '\\' && arg[1] == App.Current.ActionCharPrefix.Value) ? arg.Substring(1) : arg;
                                currentAction.Add(arg);
                            }
                        }
                    }
                }
            }
            else
            {
                var currentSubCommand = new RequestAction();
                actions.Add(currentSubCommand);

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
                            currentSubCommand.Add(arg);
                    }
                }
            }

            foreach (var action in actions)
            {
                var argsAction = action.Arguments;
                if (argsAction != null && argsAction.Length > 0)
                    action.Get = AppHelpers.ArgsToDictionary(argsAction);
            }

            return actions;
        }
    }
}
