using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class TaskCommand : Command
    {
        public TaskCommand()
        {
        }

        public string Get(string title = null, string description = null, DateTime? date = null)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            return this.GetType().Name + "." + methodSpecification;
        }

        public string Get(int? id = null)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            return this.GetType().Name + "." + methodSpecification;
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //App333.Current.Response.Write(tasks.FirstOrDefault(f=>f.Id == id));
        }

        public void Get(string title = null)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //App333.Current.Response.Write(tasks.FirstOrDefault(f => f.Title == title));
        }

        public string Save()
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            return this.GetType().Name + "." + methodSpecification;
        }

        public void Save(string title)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //var task = new Task
            //{
            //    Id = tasks.Count + 1,
            //    Title = title,
            //    DateAndTime = DateTime.Now
            //};
            //tasks.Add(task);
            //App333.Current.SaveObjectFile(tasks);
        }

        public void Save(string title, string description = null, DateTime? date = null)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);

            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //var task = new Task
            //{
            //    Id = tasks.Count + 1,
            //    Title = title,
            //    Description = description,
            //    DateAndTime = date ?? DateTime.Now
            //};
            //tasks.Add(task);
            //App333.Current.SaveObjectFile(tasks);
        }

        public void Delete(int id)
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //tasks.RemoveAll(t => t.Id == id);
            //App333.Current.SaveObjectFile(tasks);
        }

        [Action(Name="delete2")]
        public void Delete(
            [Argument(LongName="id", ShortName='i', Help="My ID", DefaultValue="not work here", ShowHelpComplement=true)]
            string id
        )
        {
            var methodMap = this.GetCurrentMethodMap();
            var methodResult = this.GetCurrentMethodResult();
            var methodSpecification = DefaultEventListener.GetMethodSpecification(methodMap);
            this.App.Console.Write(this.GetType().Name + "." + methodSpecification);
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //tasks.RemoveAll(t => t.Title == id);
            //App333.Current.SaveObjectFile(tasks);
        }
    }
}
