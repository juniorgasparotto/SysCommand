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
        public List<RequestAction> RequestActions { get; private set; }
        
        public Request(string[] args)
        {
            this.Arguments = args;
            this.GetRaw = string.Join(" ", args);
            var parser = new ArgumentsParser();
            this.Get = AppHelpers.ArgsToDictionary(args);
            this.LoadRequestActions();

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

        private void LoadRequestActions()
        {
            this.RequestActions = new List<RequestAction>();

            if (this.Arguments.Length == 0)
                return;

            if (App.Current.ActionCharPrefix == null)
                this.LoadRequestActionsWithoutPrefix();
            else
                this.LoadRequestActionsWithPrefix();

            foreach (var action in this.RequestActions)
                action.Get = AppHelpers.ArgsToDictionary(action.Arguments);
        }

        private void LoadRequestActionsWithoutPrefix()
        {
            // model: action-name --p1 "a" --p2 "b"

            var argZero = this.Arguments[0];
            var argZeroScaped = AppHelpers.RemoveScape(argZero);
            var existsAction = App.Current.Actions.Any(f => f.Name == argZeroScaped);
            var isScaped = AppHelpers.IsScaped(argZero);

            var requestAction = new RequestAction();
            requestAction.Position = 0;
            requestAction.Name = null;
            this.RequestActions.Add(requestAction);

            if (existsAction && !isScaped)
            {
                // eg: save --p1 abc
                requestAction.Name = argZero;
            }
            else
            {
                // eg: \save --p1 abc            -> add: save                    
                if (existsAction)
                    requestAction.Add(argZeroScaped);

                // eg: \save-not-exists --p1 abc -> add: \save-not-exists
                // eg: save-not-exists --p1 abc  -> add: save-not-exists
                else
                    requestAction.Add(argZero);
            }

            foreach (var arg in this.Arguments.Skip(1))
                requestAction.Add(arg);
        }

        private void LoadRequestActionsWithPrefix()
        {
            // model1: $action-name --p1 a $delete --p2 b \$other --p3 c

            var prefix = App.Current.ActionCharPrefix.Value;
            var argZero = this.Arguments[0];

            // check if "$" exists
            var argZeroHasPrefix = AppHelpers.HasCharAtFirst(argZero, prefix);

            // remove "\" if exists
            var argZeroScaped = AppHelpers.RemoveScape(argZero);

            // remove "$" if exists
            var argZeroScapedWithoutPrefix = AppHelpers.RemoveFirstCharIfFound(argZeroScaped, prefix);

            // check if "save" action exists
            var existsAction = App.Current.Actions.Any(f => f.Name == argZeroScapedWithoutPrefix);

            // if has prefix "$" and action exists
            if (argZeroHasPrefix && existsAction)
            {
                // eg: $save --p1 a

                var requestAction = new RequestAction();
                requestAction.Name = argZeroScapedWithoutPrefix;
                requestAction.Position = 0;
                this.RequestActions.Add(requestAction);

                var i = 1;
                foreach (var arg in this.Arguments.Skip(1))
                {
                    var argHasPrefix = AppHelpers.HasCharAtFirst(arg, prefix);
                    var argScaped = AppHelpers.RemoveScape(arg);
                    var argScapedWithoutPrefix = AppHelpers.RemoveFirstCharIfFound(argScaped, prefix);
                    var argExistsAction = App.Current.Actions.Any(f => f.Name == argScapedWithoutPrefix);

                    if (argHasPrefix && argExistsAction)
                    {
                        requestAction = new RequestAction();
                        requestAction.Name = argScapedWithoutPrefix;
                        requestAction.Position = i;
                        this.RequestActions.Add(requestAction);
                    }
                    else
                    {
                        var isScaped = AppHelpers.IsScaped(arg);
                        var hasPrefixInArgScaped = AppHelpers.HasCharAtFirst(argScaped, prefix);

                        // eg: \$save             -> add: $save
                        if (isScaped && hasPrefixInArgScaped && argExistsAction)
                            requestAction.Add(argScaped);
                        // eg: save               -> add: save
                        // eg: \$save-not-exists  -> add: \$save-not-exists
                        // eg: $save-not-exists   -> add: $save-not-exists
                        else
                            requestAction.Add(arg);
                    }

                    i++;
                }
            }
            else
            {
                var requestAction = new RequestAction();
                requestAction.Name = null;
                requestAction.Position = 0;
                this.RequestActions.Add(requestAction);

                var isScaped = AppHelpers.IsScaped(argZero);
                var hasPrefixInArgScaped = AppHelpers.HasCharAtFirst(argZeroScaped, prefix);

                // eg: \$save             -> add: $save
                if (isScaped && hasPrefixInArgScaped && existsAction)
                    requestAction.Add(argZeroScaped);

                // eg: --p1               -> add: --p1
                // eg: save               -> add: save
                // eg: \save              -> add: \save
                // eg: save-not-exists    -> add: save-not-exists
                // eg: \save-not-exists   -> add: \save-not-exists
                // eg: \$save-not-exists  -> add: \$save-not-exists
                // eg: $save-not-exists   -> add: $save-not-exists
                else
                    requestAction.Add(argZero);

                foreach (var arg in this.Arguments.Skip(1))
                    requestAction.Add(arg);
            }
        }

        //private RequestAction GetRequestActionDefault(int i, string arg)
        //{
        //    var prefix = App.Current.ActionCharPrefix.Value;
        //    var hasPrefix = AppHelpers.HasCharAtFirst(arg, prefix);
        //    var argScaped = AppHelpers.RemoveScape(arg);
        //    var argScapedWithoutPrefix = AppHelpers.RemoveFirstCharIfFound(argScaped, prefix);
        //    var existsAction2 = App.Current.Actions.Any(f => f.Name == argScapedWithoutPrefix);
        //    RequestAction requestAction = new RequestAction();

        //    if (hasPrefix && existsAction2)
        //    {
        //        requestAction.Name = argScapedWithoutPrefix;
        //        requestAction.Position = i;
        //    }
        //    else
        //    {
        //        if (!hasPrefix || !existsAction2)
        //            requestAction.Add(arg);
        //        else
        //            requestAction.Add(argScaped);
        //    }

        //    return requestAction;
        //}
    }
}
