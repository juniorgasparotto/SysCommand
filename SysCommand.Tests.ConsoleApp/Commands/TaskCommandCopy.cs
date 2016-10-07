using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class TaskCommandCopy : Command
    {
        public TaskCommandCopy()
        {
        }

        public void Delete()
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
        }

        [Action(Name="delete")]
        public void OtherName(int param = 0)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
        }

        public void Save()
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
        }

        public void Save(int id)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
        }

        public void Save(int id, string description = null, decimal? value = null)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
        }
    }
}
