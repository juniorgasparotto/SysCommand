using System;
namespace SysCommand.ConsoleApp
{
    public static class Strings
    {
        public static string NotFoundMessage = "Could not find any action.";
        public static string GetArgumentsInDebug = "Enter with args: ";
        public static string ArgumentAlreadyBeenSet = "The argument '{0}' has already been set";
        public static string ArgumentNotExistsByName = "The argument '{0}' does not exist";
        public static string ArgumentNotExistsByValue = "Could not find an argument to the specified value: {0}";
        public static string ArgumentIsRequired = "The argument '{0}' is required";
        public static string ArgumentHasInvalidInput = "The argument '{0}' is invalid";
        public static string ArgumentHasUnsupportedType = "The argument '{0}' is unsupported";
    }
}
