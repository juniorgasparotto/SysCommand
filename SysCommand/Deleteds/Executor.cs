//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//namespace SysCommand
//{
//    public class Executor
//    {
//        private Result<IMember> result;

//        public string[] Args { get; private set; }
//        public RunState ReturnCode { get; set; }

//        public enum InvokePriority
//        {
//            First,
//            Second,
//        }

//        public Result<IMember> Run(Result<IMember> parseResult)
//        {
//            // step2: create fluent mapping
//            //parseResult.AddRange(this.GetMainMethodsIfExists());
           
//            //// step3: execute all valid properties and after the Main methods
//            //parseResult
//            //    .With<PropertyItem>(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid))
//            //    .Invoke((int)InvokePriority.First);

//            //parseResult
//            //    .With<MethodItem>()
//            //    .Invoke((int)InvokePriority.First);

//            // step2: separate valid and invalid actions and arguments if exists
//            //this.SepareteValidAndInvalids();

//            // step3: set result code
//            //this.GetCurrentState(parseResult);

//            //// step4: configure properties and invoke main methods
//            ////results.AddRange(this.GetAllPropertiesInvokers());
//            ////results.AddRange(this.GetAllMethodMain());

//            //switch (this.ReturnCode)
//            //{
//            //    case RunState.NotFound:
//            //        this.ShowNotFound();
//            //        break;
//            //    case RunState.HasInvalidActions:
//            //        this.ShowActionsErrors();
//            //        break;
//            //    case RunState.HasInvalidArguments:
//            //        this.ShowArgumentsErrors();
//            //        break;
//            //    case RunState.Ok:
//            //        //results.AddRange(this.GetAllMethodsInvokers());
//            //        break;
//            //}

//            return parseResult;
//        }

//        //private IEnumerable<MethodItem> GetMainMethodsIfExists()
//        //{
//        //    foreach (var command in this.Commands)
//        //    {
//        //        var mainMethod = command.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
//        //        if (mainMethod != null)
//        //            yield return new MethodItem(mainMethod.Name, mainMethod.Name, command, mainMethod, null, (int)InvokePriority.First);
//        //    }
//        //}

//        //private RunState GetCurrentState(MappingResult<IMappingResultItem> results)
//        //{
//        //    // methods - exclude 'main' methods
//        //    var actions = results.With<MethodItem>(f=> f.InvokePriority == (int)InvokePriority.Second);
//        //    var actionsValids = actions.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.Valid));
//        //    var actionsInvalids = actions.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.IsInvalid));

//        //    // properties
//        //    var arguments = results.With<PropertyItem>();
//        //    var argumentsValids = arguments.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid));
//        //    var argumentsInvalids = arguments.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.IsInvalid));

//        //    // check errors
//        //    var notFoundActions = actions.Empty();
//        //    var notFoundArguments = arguments.Empty() || argumentsInvalids.All(f => f.ArgumentMapped.IsMapped == false);
//        //    var existsActionsError = actionsValids.Empty() && actionsInvalids.Any();
//        //    //var existsArgumentsError = !notFoundArguments && lstPropertiesInvokers.Count == 0 && lstInvalidsArguments.Count > 0;

//        //    if (notFoundActions && notFoundArguments)
//        //        return RunState.NotFound;
//        //    else if (existsActionsError)
//        //        return RunState.HasInvalidActions;
//        //    //else if (existsArgumentsError)
//        //    //    return ReturnCode.HasInvalidArguments;
//        //    else
//        //        return RunState.Ok;
//        //}
        
//        //private IEnumerable<MethodItem> InvokeMethodsMains()
//        //{
//        //    foreach (var method in this.GetMainMethodsIfExists())
//        //    {
//        //        method.Invoke();
//        //        yield return method;
//        //    }
//        //}
//    }
//}
