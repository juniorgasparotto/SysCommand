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

        private List<RequestAction> GetActions(string[] args)
        {
            var actions = new List<RequestAction>();
            
            if (args.Length == 0)
                return actions;

            var isAction = AppHelpers.IsActionFormat(args[0]);

            // eg: [save --p1 "a" --p2 "b"]
            if (isAction && App.Current.ActionCharPrefix == null)
            {
                var currentAction = new RequestAction();
                currentAction.Name = args[0];
                currentAction.Position = 0;
                foreach (var arg in args.Skip(1))
                    currentAction.Add(arg);
                actions.Add(currentAction);
            }            
            // eg: [$save --p1 "a" --p2 "b" $delete --p1 "a"]
            else if (isAction && App.Current.ActionCharPrefix != null)
            {
                var currentAction = default(RequestAction);
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (AppHelpers.IsActionFormat(arg))
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
            // eg: [--p1 "a" --p2 "b"]
            else if (!isAction)
            {
                var currentAction = new RequestAction();
                currentAction.Name = null;
                currentAction.Position = 0;
                foreach (var arg in args)
                {
                    // when action prefix is used and is scape: \$save
                    if (App.Current.ActionCharPrefix != null && arg.Length > 2)
                        currentAction.Add((arg[0] == '\\' && arg[1] == App.Current.ActionCharPrefix.Value) ? arg.Substring(1) : arg);
                    else
                        currentAction.Add(arg);
                }

                actions.Add(currentAction);
            }

            foreach (var action in actions)
                action.Get = AppHelpers.ArgsToDictionary(action.Arguments);

            return actions;

            //if (1==1)
            //{
            //    var currentAction = default(RequestAction);
            //    for (var i = 0; i < args.Length; i++)
            //    {
            //        var arg = args[i];
            //        if (AppHelpers.IsActionFormat(arg))
            //        {
            //            currentAction = new RequestAction();
            //            currentAction.Name = arg.Substring(1);
            //            currentAction.Position = i;
            //            actions.Add(currentAction);
            //        }
            //        else
            //        {
            //            if (currentAction != null)
            //            {
            //                if (arg.Length > 2)
            //                    arg = (arg[0] == '\\' && arg[1] == App.Current.ActionCharPrefix.Value) ? arg.Substring(1) : arg;
            //                currentAction.Add(arg);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    var currentSubCommand = new RequestAction();
            //    actions.Add(currentSubCommand);

            //    for (var i = 0; i < args.Length; i++)
            //    {
            //        var arg = args[i];
            //        if (i == 0 && !AppHelpers.IsArgumentFormat(arg))
            //        {
            //            currentSubCommand.Name = arg;
            //            currentSubCommand.Position = i;
            //        }
            //        else
            //        {
            //            if (currentSubCommand != null)
            //                currentSubCommand.Add(arg);
            //        }
            //    }
            //}
            //actions.RemoveAll(f => string.IsNullOrWhiteSpace(f.Name));

        }
    }
}
