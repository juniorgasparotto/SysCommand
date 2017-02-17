namespace Example.Input.Parser
{
    using SysCommand.ConsoleApp;
    using System;
    using System.Collections.Generic;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command1 : Command
    {
        public string Property1 { get; set; }

        public void Action1(string value = null)
        {

        }
        public void Action2(string value = null)
        {

        }

        public void Action3(string value = null)
        {
            App.Console.Write("Action3(string value = null)");
        }
    }

    public class Command2 : Command
    {
        public string Property2 { get; set; }
        public void Action1(string value = null)
        {

        }

        public void Action2(string value = null)
        {

        }

        public void Action3(int? value = null, string value2 = null)
        {
            App.Console.Write("Action3(int? value = null, string value2 = null)");
        }
    }

    public class Command3 : Command
    {
        public void Action3(List<string> value = null)
        {
            App.Console.Write("Action3(List<string> value)");
        }
    }

    public class Command4 : Command
    {
        public string Prop1 { set { App.Console.Write("Command4.Prop1"); } }
        public string Prop2 { set { App.Console.Write("Command4.Prop2"); } }
        public string Prop3 { set { App.Console.Write("Command4.Prop3"); } }
        public string Prop4 { set { App.Console.Write("Command4.Prop4"); } }

        public Command4()
        {
            this.EnablePositionalArgs = true;
        }
    }

    public class Command5 : Command
    {
        [Flags]
        public enum MyEnum
        {
            A = 1, B = 2, C = 4 , D = 8
        }

        public MyEnum Prop1
        {
            set
            {
                App.Console.Write("Command5.Prop1");
            }
        }

        public Command5()
        {
            this.EnablePositionalArgs = true;
        }
    }

    public class Command6 : Command
    {
        [Flags]
        public enum MyEnum
        {
            A = 1, B = 2, C = 4
        }

        public MyEnum Prop1
        {
            set
            {
                App.Console.Write("Command6.Prop1");
            }
        }

        public Command6()
        {
            this.EnablePositionalArgs = true;
        }
    }
}