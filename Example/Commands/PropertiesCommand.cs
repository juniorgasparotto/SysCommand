using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace Example.Commands
{
    public class TestProperty1Command : Command
    {
        public string MyProperty { get; set; }
        public int? MyPropertyInt { get; set; }
        public int MyPropertyUnsafeMode { get; set; }

        [Argument(DefaultValue = 100)]
        public int MyPropertyDefaultValue { get; set; }

        public string Main()
        {
            if (this.MyProperty != null)
                App.Console.Write("Has MyProperty");

            if (this.MyPropertyInt != null)
                App.Console.Write("Safe mode: MyPropertyInt");

            if (this.MyPropertyUnsafeMode == 0)
                App.Console.Write("Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode");

            if (this.GetPropertyResult("MyPropertyUnsafeMode") != null)
                App.Console.Write("Safe mode, but use string: MyPropertyUnsafeMode");

            if (this.GetPropertyResult(nameof(MyPropertyUnsafeMode)) != null)
                App.Console.Write("Safe mode, but only in c# 6: MyPropertyUnsafeMode");

            if (this.GetPropertyResult(nameof(MyPropertyDefaultValue)) != null)
                App.Console.Write("MyPropertyDefaultValue aways has value");

            // if necessary, add this verification to know if property had input.
            if (this.GetPropertyResult(nameof(MyPropertyDefaultValue)).ArgumentParsed.IsMapped)
                App.Console.Write("MyPropertyDefaultValue has input");

            return "Main() methods can also return values ;)";
        }
    }

    public class TestProperty2Command : Command
    {
        public bool MyCustomVerbose
        {
            set
            {
                if (value)
                    Console.WriteLine("MyCustomVerbose=true");
                else
                    App.Console.Write("MyCustomVerbose=false");
            }
        }
    }

    public class TestProperty3Command : Command
    {
        [Argument(Position = 2)]
        public int? MyPosicionalProperty1 { get; set; }

        [Argument(Position = 1)]
        public int? MyPosicionalProperty2 { get; set; }

        public TestProperty3Command()
        {
            this.EnablePositionalArgs = true;
        }

        public void Main()
        {
            if (MyPosicionalProperty1 != null)
                App.Console.Write("MyPosicionalProperty1=" + MyPosicionalProperty1);
            if (MyPosicionalProperty2 != null)
                App.Console.Write("MyPosicionalProperty2=" + MyPosicionalProperty2);
        }
    }

    public class TestProperty4Command : Command
    {
        public int? MyPropertyWithoutAttribute { get; set; }

        [Argument]
        public int? MyPropertyWithAttribute { get; set; }

        public TestProperty4Command()
        {
            this.OnlyPropertiesWithAttribute = true;
        }

        public void Main()
        {
            if (MyPropertyWithAttribute != null)
                App.Console.Write("MyPropertyWithAttribute=" + MyPropertyWithAttribute);
        }
    }

    public class TestProperty5Command : Command
    {
        [Argument(IsRequired = true)]
        public string MyPropertyRequired { get; set; }

        public void Main()
        {
            if (MyPropertyRequired != null)
                App.Console.Write("MyPropertyRequired=" + MyPropertyRequired);
        }
    }
}