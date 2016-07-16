namespace SysCommand.UnitTests
{
    public class Git
    {
        [Argument(Position = 0)]
        public string Arg0 { get; set; }

        public string a { get; set; }
        public string b { get; set; }
	
	    public string Main()
	    {
            return "Main()";
	    }
	
	    public string Main(string[] args)
	    {
		    return "Main(string[] args)";
	    }

        [Action(Ignore = true)]
	    public string Main(string a, string b)
	    {
		    return "Main(string a, string b)";
	    }
	
	    [Action(IsDefault = true)]
	    public string MethodDefault(string a = null, int? b = null)
	    {
		    return "MethodDefault(string a = null, int? b = null)";
	    }
	
	    [Action(IsDefault = true)]
	    public string MethodDefault(string a = null)
	    {
		    return "MethodDefault(string a = null)";
	    }
	
	    public string Clean(string a, string b)
	    {
		    return "Clean(string a, string b)";
	    }

	    [Action(EnablePositionalArgs=false)]
	    public string Add(string a, string b)
	    {
		    return "Add(string a, string b)";
	    }

	    [Action(UsePrefix=false, Name="commit")]
	    public string CommitAllFiles(string a, string b)
	    {
		    return "CommitAllFiles(string a, string b)";
	    }
    }
}
