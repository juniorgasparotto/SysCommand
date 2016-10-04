using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class DefaultCommand : CommandAction
    {
        public DefaultCommand()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public void Default(int p1, int p2)
        {
            App333.Current.Response.WriteLine("Executing: DefaultCommand/Default/p1/p2");
        }

        [Action(IsDefault=true)]
        public void Default2(string a)
        {
            App333.Current.Response.WriteLine("Executing: DefaultCommand/Default/a/b/c");
        }
    }
}
