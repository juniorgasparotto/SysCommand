using System.Linq;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace Example.Commands
{
    public class Method1Command : Command
    {
        public string MyAction()
        {
            return "MyAction";
        }

        public string MyAction2(int? arg0 = null, int arg1 = 0)
        {
            // recommended
            if (arg0 != null)
                App.Console.Write("arg0 has input");

            // unsafe
            if (arg1 != 0)
                App.Console.Write("arg1 wrong way to do it!");

            // safe, but bureaucratic 
            if (this.GetAction().Arguments.Any(f => f.Name == "arg1" && f.IsMapped))
                App.Console.Write("arg1 has input");

            return "MyAction2";
        }

        public string MyAction3()
        {
            return "MyAction3";
        }

        public string MyAction3(int arg0)
        {
            return "arg0 has input";
        }

        public void MyAction3(int arg0, int arg1)
        {
            App.Console.Write("arg0 has input");
            App.Console.Write("arg1 has input");
        }

        public string MyActionWithPosicional(int arg0, int arg1)
        {
            return "MyActionWithPosicional";
        }

        [Action(EnablePositionalArgs = false)]
        public string MyActionWithoutPosicional(int arg0, int arg1)
        {
            return "MyActionWithoutPosicional";
        }
    }

    public class Method2Command : Command
    {
        public Method2Command()
        {
            this.OnlyMethodsWithAttribute = true;
        }

        [Action]
        public string MyActionWithAttribute()
        {
            return "MyActionWithAttribute";
        }

        public string MyActionWithoutAttribute()
        {
            return "MyActionWithAttribute";
        }
    }

    public class Method3Command : Command
    {
        public string MyActionNotIgnored()
        {
            return "MyActionNotIgnored";
        }

        [Action(Ignore = true)]
        public string MyActionIgnored()
        {
            return "MyActionIgnored";
        }
    }

    public class Method4Command : Command
    {
        [Action(Name = "ActionNewName")]
        public string MyAction(
            [Argument(LongName = "longName1", ShortName = 'a')]
            string arg0, // customized with long and short option
            string arg1, // only with long option
            [Argument(ShortName = 'w')]
            string arg2, // only with short option
            string z     // only with short option
        )
        {
            return "ActionNewName";
        }

        [Action(Name = "ActionNewName2")]
        public string MyAction2(
            [Argument(LongName = "longName1", ShortName = 'a', Position = 2)]
            string arg0,
            [Argument(LongName = "longName2", ShortName = 'b', Position = 1)]
            string arg1
        )
        {
            return "ActionNewName2";
        }
    }

    public class Method5Command : Command
    {
        public string MyActionWithArgsInverted(
            [Argument(Position = 2)]
            string arg0,
            [Argument(Position = 1)]
            string arg1
        )
        {
            return $"arg0 = '{arg0}'; arg1 = '{arg1}'";
        }
    }

    public class PrefixedCommand : Command
    {
        public PrefixedCommand()
        {
            this.UsePrefixInAllMethods = true;
        }

        public string MyAction()
        {
            return "prefixed-my-action";
        }

        [Action(Name = "my-action2-custom")]
        public string MyAction2()
        {
            return "prefixed-my-action2-custom";
        }

        [Action(UsePrefix = false)]
        public string MyActionWithoutPrefix()
        {
            return "my-action-without-prefix";
        }
    }

    public class Prefixed2Command : Command
    {
        public Prefixed2Command()
        {
            this.PrefixMethods = "custom-prefix";
            this.UsePrefixInAllMethods = true;
        }

        public string MyAction()
        {
            return "custom-prefix-my-action";
        }
    }

    public class MethodHelpCommand : Command
    {
        public MethodHelpCommand()
        {
            this.HelpText = "Help for this command";
        }

        [Action(Help = "Action help")]
        public string MyActionHelp(
            [Argument(Help = "Argument help")]
            string arg0, // With complement

            [Argument(Help = "Argument help", ShowHelpComplement = false)]
            string arg1  // Without complement
        )
        {
            return "Action help";
        }
    }
}