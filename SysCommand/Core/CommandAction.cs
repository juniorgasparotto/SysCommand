using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public abstract class CommandAction
    {
        private List<ActionInstance> actions;
        public IEnumerable<ActionInstance> Actions { get { return actions;  } }
        public bool AllowSaveArgsInStorage { get; protected set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool AddPrefixInActions { get; set; }
        public string PrefixActions { get; set; }

        public CommandAction()
        {
            this.actions = new List<ActionInstance>();
        }

        public virtual void Setup()
        {
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic).ToList();
            var fluentCmdParserSetup = new FclpActionArgumentSetup();
            foreach (var method in methods)
            {
                var setup = new ActionSetup();
                var action = setup.CreateAction(this, method);
                if (action != null)
                {
                    this.actions.Add(action);
                    foreach (var arg in action.ActionArguments)
                        fluentCmdParserSetup.Setup(arg);
                }
            }

            if (this.actions.Count(f => f.IsDefault) != this.actions.Count)
                throw new Exception("You can not coexist standard actions and named actions.");
        }

        public virtual void Parse()
        {
            foreach (var action in this.Actions)
            {
                // 1) action.RequestAction != null: The action sent in input has match with action defined in current method.
                // 1.1) The number of input parameters must be equal or greater than the amount of required method parameters 
                //    and can not exceed the total amount of the method parameters.
                // 1.1.1) action.RequestAction.Get.Count: Count input
                // 1.1.2) action.ParametersParseds.Count: Count parseds input
                // 2) action.RequestAction && action.IsDefault: Any action is sent and the action is defined has default.

                var methodParameters = action.MethodInfo.GetParameters();
                var countTotal = methodParameters.Length;
                var countRequired = methodParameters.Count(f => !f.IsOptional);
                var actionIsCanditade = action.RequestAction != null && (action.RequestAction.Get.Count >= countRequired && action.RequestAction.Get.Count <= countTotal);
                var actionIsDefault = action.RequestAction == null && action.IsDefault;

                if (actionIsCanditade || actionIsDefault)
                {
                    string[] args;

                    if (action.RequestAction != null)
                        args = action.RequestAction.Arguments;
                    else if (actionIsDefault && App.Current.Request.Actions.Count == 0)
                        args = App.Current.Request.Arguments;
                    else
                        args = new string[0];

                    //var parameters = action.MethodInfo.GetParameters();
                    //for(var i = 0; i < parameters.Length; i++)
                    //{
                    //    if (!App.Current.Request.IsArgumentFormat(args[i]))
                    //    {

                    //    }
                    //}

                    action.Result = action.Parser.Parse(args);
                    var countArgsParsed = action.ActionArguments.Count(f => f.HasParsed);
                    if (action.Result.HasErrors == false && methodParameters.Length == countArgsParsed)
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

        private List<ActionInstance> GetActionsToExecute()
        {
            // Run only the action that has fewest amount of parameters in case of parse duplicity.
            var actions = this.Actions.ToList();
            var duplicates = actions.Where(f => f.HasParsed).GroupBy(f => f.Name).Where(f => f.Count() > 1);
            foreach (var groupSameName in duplicates)
            {
                // just keep the action that has more correspondence with input.
                var list = groupSameName.OrderBy(f => f.Result.UnMatchedOptions.Count()).ToList();
                list.Remove(list.FirstOrDefault());

                foreach (var actionRemove in list)
                    actions.Remove(actionRemove);
            }
            return actions;
        }

    }
}
