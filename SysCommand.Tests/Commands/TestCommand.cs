using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace SysCommand.Tests
{
    public class TestCommand : Command<TestCommand.Arguments>
    {
        public override void Execute()
        {
            App.Current.GetOrCreateObjectFile<AppConfig>().LastExecuteDate = DateTime.Now;
            App.Current.SaveObjectFile<AppConfig>(App.Current.GetOrCreateObjectFile<AppConfig>());

            ConsoleWriter.Info("Info");
            ConsoleWriter.Info("Warning");
            ConsoleWriter.Error("Error");
            ConsoleWriter.Success("Success");
            ConsoleWriter.Question("Question?");
            ConsoleWriter.Info("Test: " + this.Args.Test);
            ConsoleWriter.Info("Test2: " + this.Args.Test2);
            ConsoleWriter.Info("Test3: " + this.Args.Test3);
        }

        #region Internal Parameters
        public class Arguments : IHelp
        {
            [CommandPropertyAttribute(ShortName = 't')]
            public int Test { get; set; }

            [CommandPropertyAttribute()]
            public string Test2 { get; set; }


            [CommandPropertyAttribute(LongName = "test3", Help = "test3. Simple help", Default = "default: abc")]
            public string Test3 { get; set; }

            public string Test4 { get; set; }

            #region IHelp
            public string GetHelp(string propName)
            {
                if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Test).Name)
                    return string.Format("Test example. Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.Test));
                else if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Test2).Name)
                    return string.Format("Test2 example: Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.Test2));
                return null;
            }
            #endregion
        }
        #endregion
    }
}
