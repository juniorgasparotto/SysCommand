using SysCommand;
using SysCommand.ConsoleApp;
using SysCommand.Tests.ConsoleApp.Commands;
using System;
using System.Collections.Generic;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            bool lastBreakLineInNextWrite = false;
            while (true)
            {
                var app = new App();
                app.Run();
                lastBreakLineInNextWrite = app.Console.BreakLineInNextWrite;

                if (!App.IsDebug)
                    return app.Console.ExitCode;
            }
        }
    }

//""
//  main() *
//	default()

//"save --show-time delete --show-time"
//    main()
//    save()
//    main()
//    delete()
	
//"save delete"
//    main()
//    save()
//    main()
//    delete()

//"--show-time"
//    main()
	
//"--show-time save"
//    main()
//    save()

//"value"
//    main()
//    main(string v)
}
