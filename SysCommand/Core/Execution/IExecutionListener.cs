using System;

namespace SysCommand
{
    public interface IExecutionListener
    {
        void OnError(string[] args, Map map, Result<IMember> parseResult, Exception ex);
        void OnSuccess(string[] args, Map map, Result<IMember> result);
        //void OnArgsIsEmpty(string[] args, Map map, Result<IMember> result);
        void OnNotFound(string[] args, Map map, Result<IMember> result);
        void OnInvalidArgumentParse(string[] args, Map map, Result<IMember> result);

        //public virtual void ShowActionsErrors()
        //{
        //    foreach (var action in this.ActionsInvalids)
        //    {
        //        foreach (var arg in action.ArgumentsMapped)
        //        {
        //            Console.WriteLine("Error in action: {0}", action.ToString());
        //            Console.WriteLine("- {0}", this.GetArgumentMappedErrorDescription(arg));
        //        }
        //    }
        //}

        //public virtual void ShowArgumentsErrors()
        //{
        //    foreach (var arg in this.ArgumentsInvalids)
        //        Console.WriteLine("Error in argument: {0} - {1}", arg.Name, this.GetArgumentMappedErrorDescription(arg));
        //}

        //public virtual void ShowNotFound(string[] args)
        //{
        //    var maxLength = 40;
        //    var command = string.Join(" ", args);
        //    command = command.Length <= maxLength ? command : command.Substring(0, maxLength);
        //    Console.WriteLine("The command '{0}' was not found", command);
        //}

        //public virtual void ShowHelp()
        //{
        //    //var dic = new Dictionary<string, string>();
        //    //foreach (var cmd in this.Commands2)
        //    //{
        //    //    foreach (var opt in cmd.Parser.Options)
        //    //    {
        //    //        var key = "";
        //    //        if (!string.IsNullOrWhiteSpace(opt.ShortName) && !string.IsNullOrWhiteSpace(opt.LongName))
        //    //            key = "-" + opt.ShortName + ", --" + opt.LongName;
        //    //        else if (!string.IsNullOrWhiteSpace(opt.ShortName))
        //    //            key = "-" + opt.ShortName;
        //    //        else if (!string.IsNullOrWhiteSpace(opt.LongName))
        //    //            key = "--" + opt.LongName;

        //    //        dic[key] = opt.Description;
        //    //    }
        //    //}

        //    //if (dic.Count > 0)
        //    //    Console.WriteLine(AppHelpers.GetConsoleHelper(dic, 4));
        //}

        //public string GetArgumentMappedErrorDescription(ArgumentMapped argumentMapped)
        //{
        //    if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentAlreadyBeenSet))
        //        return string.Format("The argument '{0}' has already been set", argumentMapped.GetArgumentNameInputted());
        //    else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByName))
        //        return string.Format("The argument '{0}' does not exist", argumentMapped.GetArgumentNameInputted());
        //    else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByValue))
        //        return string.Format("Could not find an argument to the specified value: {0}", argumentMapped.Raw);
        //    else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsRequired))
        //        return string.Format("The argument '{0}' is required", argumentMapped.GetArgumentNameInputted());
        //    else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentHasInvalidInput))
        //        return string.Format("The argument '{0}' is invalid", argumentMapped.GetArgumentNameInputted());
        //    else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentHasUnsupportedType))
        //        return string.Format("The argument '{0}' is unsupported", argumentMapped.GetArgumentNameInputted());
        //    return null;
        //}
    }
}