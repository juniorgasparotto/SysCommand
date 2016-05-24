using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public abstract class Command2
    {
        public List<ActionDefinition> Actions;
        
        public bool AllowSaveArgsInStorage { get; protected set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }

        public class ActionDefinition
        {
            public ActionAttribute Attribute { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public Dictionary<ParameterInfo, object> ParametersValues { get; set; }
            public FluentCommandLineParser Parser { get; set; }
            public ICommandLineParserResult Result { get; set; }

            public string Name { get; set; }
            public SubCommand ActionInput { get; set; }
            public Command2 Object { get; set; }
            public bool HasParsed { get; set; }
        }

        public Command2()
        {
            this.Actions = new List<ActionDefinition>();
        }

        public virtual void Parse()
        {
            if (this.Actions.Count > 0)
                return;

            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic).ToList();
            foreach (var method in methods)
            {
                var methodAttr = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;
                var actionName = "";

                if (methodAttr != null && !string.IsNullOrWhiteSpace(methodAttr.ActionName))
                    actionName = methodAttr.ActionName;
                else
                    actionName = AppHelpers.ToLowerSeparate(method.Name, '-');

                var actionInput = App.Current.Request.SubCommands.FirstOrDefault(f => f.Name == actionName);
                var action = new ActionDefinition()
                {
                    Attribute = methodAttr,
                    Name = actionName,
                    Parser = new FluentCommandLineParser(),
                    ParametersValues = new Dictionary<ParameterInfo, object>(),
                    MethodInfo = method,
                    ActionInput = actionInput,
                    Object = this,
                };

                this.Actions.Add(action);

                var autoFill = new CommandMethodAutoFill(action);
                autoFill.AutoSetup();

                string[] args = actionInput != null ? actionInput.Arguments.ToArray() : new string[0];
                action.Result = action.Parser.Parse(args);

                if (action.Result.HasErrors == false && method.GetParameters().Length == action.ParametersValues.Count)
                {
                    action.HasParsed = true;

                    // Only update 'Args.Command' and set the args in config if has success
                    if (this.AllowSaveArgsInStorage)
                    {
                    }
                }
            }
        }

        public virtual void Execute()
        {
            foreach(var action in this.Actions)
            {
                if (action.HasParsed)
                {
                    var dic = action.ParametersValues.ToDictionary(k => k.Key.Name, v => v.Value);
                    action.MethodInfo.InvokeWithNamedParameters(this, dic);
                }
            }
        }
    }
}
