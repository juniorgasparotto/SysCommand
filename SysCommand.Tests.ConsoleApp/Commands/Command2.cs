using System;
using System.Linq;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.Parsing;
using SysCommand.Mapping;
using SysCommand.Execution;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class Command2 : Command
    {
        public string Title { get; set; }

        public Command2()
        {
            this.EnablePositionalArgs = true;
        }

        public string Main()
        {
            return this.GetType().Name + ".Main";
        }

        [Action(IsDefault = true)]
        public string Default()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        [Action(IsDefault = true)]
        public string Default2()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Get(string title = null, string description = null, DateTime? date = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Get(int? id = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //App333.Current.Response.Write(tasks.FirstOrDefault(f=>f.Id == id));
        }

        public string Get(string title = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
            //var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            //App333.Current.Response.Write(tasks.FirstOrDefault(f => f.Title == title));
        }

        public string Save()
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        public string Save(string title)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
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

        public string Save(string title, string description = null, DateTime? date = null)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());

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

        public string Delete(int id)
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        [Action(Name="delete2")]
        public string Delete(
            [Argument(LongName="id", ShortName='i', Help="My ID", DefaultValue="not work here", ShowHelpComplement=true)]
            string id
        )
        {
            return GetDebugName(this.CurrentMethodMap(), this.CurrentMethodParse());
        }

        private string GetDebugName(ActionMap map, MethodResult result)
        {
            if (map != result.ActionParsed.ActionMap)
                throw new Exception("There are errors in one of the methods: GetCurrentMethodMap() or GetCurrentMethodResult()");

            var specification = App.MessageFormatter.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}
