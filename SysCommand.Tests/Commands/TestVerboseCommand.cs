using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace SysCommand.Tests
{
    public class TestVerboseCommand : CommandArguments<TestVerboseCommand.Arguments>
    {
        public override void Execute()
        {
            App.Current.Response.WriteLine("Executing: TestVerboseCommand - aways executed because has default value in property 'Test3'");
            if (this.ArgsObject.TestVerbose)
            {
                App.Current.GetOrCreateObjectFile<AppConfig>().LastExecuteDate = DateTime.Now;
                App.Current.SaveObjectFile<AppConfig>(App.Current.GetOrCreateObjectFile<AppConfig>());

                App.Current.Response.Info(string.Format("Info '{0}'", (int)VerboseEnum.Info));
                App.Current.Response.Success(string.Format("Success '{0}'", (int)VerboseEnum.Success));
                App.Current.Response.Critical(string.Format("Critical '{0}'", (int)VerboseEnum.Critical));
                App.Current.Response.Warning(string.Format("Warning '{0}'", (int)VerboseEnum.Warning));
                App.Current.Response.Error(string.Format("Error '{0}'", (int)VerboseEnum.Error));
                App.Current.Response.Question(string.Format("Question? '{0}'", (int)VerboseEnum.Question));
            }
        }

        #region Internal Parameters
        public class Arguments : IHelp
        {
            [ArgumentAttribute(ShortName = 't')]
            public bool TestVerbose { get; set; }

            [ArgumentAttribute()]
            public string Test2 { get; set; }

            [ArgumentAttribute(LongName = "test3", Help = "test3. Simple help", Default = "default: abc", ShowHelpComplement = true)]
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
