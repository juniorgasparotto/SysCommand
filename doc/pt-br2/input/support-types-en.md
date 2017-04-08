## Tipos suportados <header-set anchor-name="support-types" />

string bool decimal double int uint DateTime byte short ushort long ulong float char Enum Enum with Flags Generic collections (IEnumerable, IList, ICollection) Arrays

Syntax

[action-name ][-|/|--][argument-name][=|:| ][value]

Boolean syntax

MyApp.exe -a // true MyApp.exe -a- // false MyApp.exe -a+ // true MyApp.exe -a - // false MyApp.exe -a + // true MyApp.exe -a true // true MyApp.exe -a false // false MyApp.exe -a 0 // true MyApp.exe -a 1 // false

Multiple assignments syntax

MyApp.exe -abc // true for a, b and c MyApp.exe -abc- // false for a, b and c MyApp.exe -abc+ // true for a, b and c

Enum syntax

[Flags] public enum Verbose { None = 0, All = 1, Info = 2, Success = 4, Critical = 8, Warning = 16, Error = 32, Quiet = 64 }

MyApp.exe --verbose Error Info Success MyApp.exe --verbose 32 2 Success

Generic collections or Array sintax

public void MyAction(IEnumerable<decimal> myLst, string[] myArray = null);

MyApp.exe --my-lst 1.0 1.99 MyApp.exe 1.0 1.99 // positional MyApp.exe --my-lst 1.0 1.99 --my-array str1 str2 MyApp.exe 1.0 1.99 str1 str2 // positional

Importante!

Todos as conversões levam em consideração a cultura configurada na propriedade estática "CultureInfo.CurrentCulture".

</decimal>