//using SysCommand.Parser;
//using System.Collections.Generic;
//using System.Linq;
//using System;

//namespace SysCommand
//{
//    public class CommandParse
//    {
//        public List<ActionMapped> Methods { get; set; }
//        public List<ArgumentMapped> Properties { get; set; }
//        public List<ActionMapped> MethodsInvalid { get; set; }
//        public List<ArgumentMapped> PropertiesInvalid { get; set; }
//        //public Result<IMember> Result { get; internal set; }

//        public bool HasError
//        {
//            get
//            {
//                return this.MethodsInvalid.Any() || this.PropertiesInvalid.Any();
//            }
//        }

//        public bool IsValid
//        {
//            get
//            {
//                return !HasError && this.Methods.Any() || this.Properties.Any();
//            }
//        }

//        public CommandBase Command { get; internal set; }

//        public CommandParse()
//        {
//            this.Methods = new List<ActionMapped>();
//            this.MethodsInvalid = new List<ActionMapped>();
//            this.PropertiesInvalid = new List<ArgumentMapped>();
//            this.Properties = new List<ArgumentMapped>();
//            //this.Result = new Result<IMember>();
//        }
//    }
    
//}