using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace SysCommand.Tests
{
    public class TestVerboseCommand : Command<TestVerboseCommand.Arguments>
    {
        public override void Execute()
        {
            App.Current.GetOrCreateObjectFile<AppConfig>().LastExecuteDate = DateTime.Now;
            App.Current.SaveObjectFile<AppConfig>(App.Current.GetOrCreateObjectFile<AppConfig>());

            ConsoleWriter.Info(string.Format("Info '{0}'", (int)VerboseEnum.Info));
            ConsoleWriter.Success(string.Format("Success '{0}'", (int)VerboseEnum.Success));
            ConsoleWriter.Critical(string.Format("Critical '{0}'", (int)VerboseEnum.Critical));
            ConsoleWriter.Warning(string.Format("Warning '{0}'", (int)VerboseEnum.Warning));
            ConsoleWriter.Error(string.Format("Error '{0}'", (int)VerboseEnum.Error));
            ConsoleWriter.Question(string.Format("Question? '{0}'", (int)VerboseEnum.Question));
        }

        #region Internal Parameters
        public class Arguments : IHelp
        {
            [CommandPropertyAttribute(ShortName = 't')]
            public bool TestVerbose { get; set; }

            [CommandPropertyAttribute()]
            public string Test2 { get; set; }

            [CommandPropertyAttribute(LongName = "test3", Help = "test3. Simple help", Default = "default: abc")]
            public string Test3 { get; set; }

            public string Test4 { get; set; }

            #region IHelp
            public string GetHelp(string propName)
            {
                if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.TestVerbose).Name)
                    return string.Format("Test example. Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.TestVerbose));
                else if (propName == AppHelpers.GetPropertyInfo<Arguments>(f => f.Test2).Name)
                    return string.Format("Test2 example: Default is '{0}'", CommandStorage.GetValueForArgsType<Arguments>(f => f.Test2));
                return null;
            }
            #endregion
        }
        #endregion
    }
}
