using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SysCommand
{
    public class FclpActionArgumentSetup
    {
        public void Setup(ActionArgumentInstance actionArgument)
        {
            var helperType = typeof(FluentCommandLineParserSetupHelper<>).MakeGenericType(actionArgument.ParameterInfo.ParameterType);
            var helper = (FclpActionArgumentSetup)Activator.CreateInstance(helperType);
            helper.SetupTyped(actionArgument);
        }

        protected virtual void SetupTyped(ActionArgumentInstance actionArgumentInstance) { }

        // A subclass with a static type parameter
        private class FluentCommandLineParserSetupHelper<TParameter> : FclpActionArgumentSetup
        {
            public FluentCommandLineParserSetupHelper()
            {
            }

            protected override void SetupTyped(ActionArgumentInstance actionArgument)
            {
                ICommandLineOptionFluent<TParameter> setup;
                if (actionArgument.ShortName != default(char) && !string.IsNullOrWhiteSpace(actionArgument.LongName))
                    setup = actionArgument.Action.Parser.Setup<TParameter>(actionArgument.ShortName, actionArgument.LongName);
                else if (actionArgument.ShortName != default(char))
                    setup = actionArgument.Action.Parser.Setup<TParameter>(actionArgument.ShortName);
                else if (!string.IsNullOrWhiteSpace(actionArgument.LongName))
                    setup = actionArgument.Action.Parser.Setup<TParameter>(actionArgument.LongName);
                else
                    throw new Exception("No argument name (shortName or longName) was specify.");

                if (actionArgument.HasDefault)
                    setup.SetDefault((TParameter)actionArgument.DefaultValue);
                else
                {
                    //if (this.parent.action.RequestAction != null)
                    setup.Required();
                }

                if (!string.IsNullOrWhiteSpace(actionArgument.Help))
                    setup.WithDescription(actionArgument.Help);

                setup.Callback
                (
                    value =>
                    {
                        //if (value != null && !value.Equals(actionArgument.DefaultValue))
                        //{
                            //if (!string.IsNullOrWhiteSpace(actionArgument.LongName))
                            //    this.parent.CommandsParseds["--" + actionArgument.LongName] = value.ToString();
                            //else
                            //    this.parent.CommandsParseds["-" + actionArgument.ShortName.ToString()] = value.ToString();
                            //actionArgument.HasParsed = true;
                        //}

                        actionArgument.HasParsed = true;
                        actionArgument.Value = value;
                        //this.parent.action.ParametersParseds[parameter] = value;
                    }
                );
            }
        }
    }
}