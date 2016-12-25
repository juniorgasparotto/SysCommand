#SysCommand
Framework for development applications using the MVC pattern.

#Install

* NuGet: 
* NuGet Core CRL: 

##Features

  * Console Application with MVC
  *  Razor templates: Just use the return "Command.View()" in your actions, like MVC Web application. (using System.Web.Razor dependency)
  ** T4 templates: Just use the return "Command.ViewT4()" in your actions.
  ** Indented text using the class "TableView".
  ** Functionality Multi Action to be possible invoke several actions in the same input. By default is enable 'App.EnableMultiAction'.
  * Automatic configuration. Just the class inherit from "Command".
  * Automatic help functionality including usage mode. Just use the input actions 'help'
  * Functionality for saving command histories. Just use the input actions 'history-save [name]', 'history-load [name]', 'history-remove [name]' and 'history-list'
  * Simple mechanism of object persistence in JSON text files (using NewtonSoft dependency)
  * Mechanism to speed development in debug mode. Just use the "App.RunInfiniteIfDebug()" method.
  ** Include the command 'clear' to clear the console window when in debug mode.
  * Mechanism to help write and read informations: Just use the console wrapper "App.Console":
  ** Write: Print texts using the following verbs: "Info", "Success", "Warning", "Critical", "Error", "None", "All".
  ** Read: If you use the 'Writes' methods is recommended use the reads methods.
  ** Verbose: Choose which are verbs can be printed in console. Just use the input argument '-v' or '--verbose'
  * Functionality to persists anything in App scope (in memory). Just use 'App.Items".
  * Events controllers "OnComplete", "OnException" e etc...
  * Extras: Simple command line parser using "OptionSet" class.

##Tutorials

```

 -p, --path=VALUE           Set the file path to convert to SQL.
      --delimiter=VALUE      Set the delimiter columns, default is ';'.
      --count=VALUE          Set the count line to generate
      --dbname=VALUE         Set the database name to determine the type of
                               output SQL, the options are:
                                [sqlserver].
      --tname=VALUE          Set the table name to generate, default is '#CSV'.
      --maxbulk=VALUE        Set the amount of 'values' that will be grouped in
                               'inserts' section, default is '25'.
      --insert-format=VALUE  Set the output format to 'insert values', default
                               is 'None' and the options are:
                                [none],
                                [break-line],
                                [break-line-and-show-columns]
      --not-header           Set if the CSV hasen't header. automatically creates with format 'Field[n]'
  -v                         Increase debug
  -h, -?, --help             Show the help.

```

#Example

```
 C:\CsvToSql.exe -path "C:/in.csv" > C:\output.sql
 C:\CsvToSql.exe -path "C:/in.csv" -delimiter ";" > C:\output.sql
 C:\CsvToSql.exe -path "C:/in.csv" -delimiter ";" -not-header > C:\output.sql 
 C:\CsvToSql.exe -path "C:/in.csv" -delimiter ";" -tname "myTable" > C:\output.sql
 C:\CsvToSql.exe -p "C:/in.csv" -delimiter ";" -tname "myTable" -maxbulk 1 > C:\output.sql
 C:\CsvToSql.exe -p "C:/in.csv" -count 10 -delimiter ";" -insert-format break-line > C:\output.sql
 C:\CsvToSql.exe -p "C:/in.csv" -count 10 -delimiter ";" -insert-format break-line-and-show-columns > C:\output.sql
 
 printf "field1,field2,field3,field4\n1,2,3,4" | csvtosql.exe -delimiter ","
 
``` 

**Open "C:\output.sql" file to show the content or omit the command "... > C:\output.sql" to show in console.**
 
## Contributors
 * [juniorgasparotto](https://github.com/juniorgasparotto)
