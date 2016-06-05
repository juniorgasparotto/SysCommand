using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SysCommand
{
    public class ActionSetup
    {
        public ActionInstance CreateAction(CommandAction command, MethodInfo method)
        {
            var action = new ActionInstance();
            var methodParameters = method.GetParameters();
            var methodAttr = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;
            if (methodAttr != null && methodAttr.Ignore)
                return null;

            var actionName = "";
            if (methodAttr != null && !string.IsNullOrWhiteSpace(methodAttr.Name))
                actionName = methodAttr.Name;
            else
                actionName = AppHelpers.ToLowerSeparate(method.Name, '-');

            var addPrefixInCurrentAction = methodAttr == null ? true : methodAttr.AddPrefix;
            if (command.AddPrefixInActions && addPrefixInCurrentAction)
                actionName = this.AddPrefixAction(actionName, command.PrefixActions);

            var hasDefaultAction = false;
            if (method.Name.ToLower() == "default" || (methodAttr != null && methodAttr.IsDefault == true))
                hasDefaultAction = true;

            action.Name = actionName;
            action.Attribute = methodAttr;
            action.Parser = new FluentCommandLineParser();
            action.MethodInfo = method;
            action.Object = command;
            action.IsDefault = hasDefaultAction;

            this.CreateActionArguments(action);
            return action;
        }

        private void CreateActionArguments(ActionInstance action)
        {
            foreach (var parameter in action.MethodInfo.GetParameters())
            {
                var argument = this.CreateActionArgument(action, parameter);
                action.ActionArguments.Add(argument);
            }
        }

        private ActionArgumentInstance CreateActionArgument(ActionInstance action, ParameterInfo parameter)
        {
            var attribute = Attribute.GetCustomAttribute(parameter, typeof(ArgumentAttribute)) as ArgumentAttribute;

            var argument = new ActionArgumentInstance
            {
                ParameterInfo = parameter,
                Action = action,
                ArgumentAttribute = attribute,
                Name = parameter.Name,
                DefaultValue = !(parameter.DefaultValue is DBNull) ? parameter.DefaultValue : null,
                HasDefault = !(parameter.DefaultValue is DBNull),
                //ShortName = attribute == null ? default(char) : attribute.ShortName,
                //LongName = attribute == null ? null : attribute.LongName,
            };

            if (attribute == null)
            {
                attribute = new ArgumentAttribute();
                if (parameter.Name.Length == 1)
                    argument.ShortName = parameter.Name[0];
                else
                    argument.LongName = parameter.Name;
            }
            else
            {
                argument.ShortName = attribute.ShortName;
                argument.LongName = attribute.LongName;
            }

            var help = default(string);
            if (!string.IsNullOrWhiteSpace(attribute.Help))
                help = attribute.Help;
            else if (action.Object is IHelp)
                help = ((IHelp)action.Object).GetHelp(parameter.Name);

            if (argument.HasDefault)
            {
                if (attribute.ShowHelpComplement)
                    help = this.ConcatHelpWithDefaultValue(help, argument.DefaultValue);
            }
            else
            {
                if (attribute.ShowHelpComplement)
                    help = this.ConcatHelpWithRequired(help);
            }

            if (!string.IsNullOrWhiteSpace(help))
                argument.Help = help;

            return argument;
        }

        private string AddPrefixAction(string actionName, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = AppHelpers.ToLowerSeparate(this.GetType().Name, '-');
                var split = prefix.Split('-').ToList();
                if (split.Last() == "command")
                {
                    split.RemoveAt(split.Count - 1);
                    prefix = string.Join("-", split.ToArray());
                }
            }

            return prefix + "-" + actionName;
        }

        private string ConcatHelpWithDefaultValue(string help, object defaultValue)
        {
            return AppHelpers.ConcatFinalPhase(help, "Is optional (default \"" + defaultValue + "\").");
        }

        private string ConcatHelpWithRequired(string help)
        {
            return AppHelpers.ConcatFinalPhase(help, "Is required.");
        }

        //public string GetCommandsParsed()
        //{
        //    var argsUseds = default(string);
        //    foreach (var arg in this.CommandsParseds)
        //    {
        //        var command = string.Format("{0} \"{1}\"", arg.Key, arg.Value.Replace("\\\"", "\\\"").Replace("'", "\'"));
        //        argsUseds += (argsUseds == null) ? command : " " + command;
        //    }

        //    return argsUseds;
        //}
    }
}