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

        public static string HelpUsageMethodsLabel = "actions";
        public static string HelpUsageMethodsParamsLabel = "args";
        public static string HelpUsageLabel = "usage:";
        public static string HelpUsageActionLabel = "usage: {0}";
        public static string HelpUsageNumberLabel = "number";
        public static string HelpUsageLetterLabel = "letter";
        public static string HelpUsagePhraseLabel = "phrase";
        public static string HelpUsageListOfLabel = "list of {0}";
        public static string HelpArgDescOptionalWithDefaultValue = "Is optional (default <{0}>).";
        public static string HelpArgDescOptionalWithoutDefaultValue = "Is optional.";
        public static string HelpArgDescRequired = "Is required.";
        public static string HelpFooterDesc = "Use 'help --action=<name>' to view the details of any action. Every action with the symbol \"*\" can have his name omitted.";
        public static string HelpEmpty = "No help information was found.";
        public static string HelpNoActionFound = "No action was found.";

        public static string ErrorInvalidMethod = "Error in method: {0}";
        public static string ErrorInCommand = "There are errors in command: {0}";

    }
}
