using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public abstract class CommandAction
    {
        public List<InstanceAction> Actions;
        public bool AllowSaveArgsInStorage { get; protected set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }

        public class InstanceAction
        {
            public string Name { get; set; }
            public RequestAction RequestAction { get; set; }
            public CommandAction Object { get; set; }
            public ActionAttribute Attribute { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public Dictionary<ParameterInfo, object> ParametersParseds { get; set; }
            public FluentCommandLineParser Parser { get; set; }
            public ICommandLineParserResult Result { get; set; }
            public bool HasParsed { get; set; }
        }

        public CommandAction()
        {
            this.Actions = new List<InstanceAction>();
        }

        public virtual void Parse()
        {
            if (this.Actions.Count > 0)
                return;

            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic).ToList();
            foreach (var method in methods)
            {
                var methodParameters = method.GetParameters();
                var methodAttr = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;
                var actionName = "";

                if (methodAttr != null && !string.IsNullOrWhiteSpace(methodAttr.ActionName))
                    actionName = methodAttr.ActionName;
                else
                    actionName = AppHelpers.ToLowerSeparate(method.Name, '-');

                var requestAction = App.Current.Request.Actions.FirstOrDefault(f => f.Name == actionName);
                var action = new InstanceAction()
                {
                    Name = actionName,
                    RequestAction = requestAction,
                    Attribute = methodAttr,
                    Parser = new FluentCommandLineParser(),
                    ParametersParseds = new Dictionary<ParameterInfo, object>(),
                    MethodInfo = method,
                    Object = this,
                };

                this.Actions.Add(action);

                var autoFill = new CommandMethodAutoFill(action);
                autoFill.AutoSetup();

                // action.ActionInput.Get: No parsed with method, simple get
                // action.ParametersParseds: parameters parsed with method
                if (action.RequestAction != null && action.RequestAction.Get.Count == methodParameters.Count(f => !f.IsOptional))
                {
                    string[] args = requestAction != null ? requestAction.Arguments : new string[0];
                    action.Result = action.Parser.Parse(args);

                    if (action.Result.HasErrors == false && methodParameters.Length == action.ParametersParseds.Count)
                    {
                        action.HasParsed = true;

                        // Only update 'Args.Command' and set the args in config if has success
                        if (this.AllowSaveArgsInStorage)
                        {
                        }
                    }
                }
            }
        }

        public virtual void Execute()
        {
            var actions = this.GetActionsToExecute();
            foreach (var action in actions)
            {
                if (action.HasParsed)
                {
                    var dic = action.ParametersParseds.ToDictionary(k => k.Key.Name, v => v.Value);
                    action.MethodInfo.InvokeWithNamedParameters(this, dic);
                }
            }
        }

        private List<InstanceAction> GetActionsToExecute()
        {
            // Run only the action that has fewest amount of parameters in case of parse duplicity.
            var actions = this.Actions.ToList();
            var duplicates = actions.Where(f => f.HasParsed).GroupBy(f => f.Name).Where(f => f.Count() > 1);
            foreach (var groupSameName in duplicates)
            {
                var list = groupSameName.OrderBy(f => f.MethodInfo.GetParameters().Length).ToList();
                list.Remove(list.FirstOrDefault());

                foreach (var actionRemove in list)
                    actions.Remove(actionRemove);
            }
            return actions;
        }
    }
}
