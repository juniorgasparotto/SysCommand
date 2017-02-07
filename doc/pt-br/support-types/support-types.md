# Tipos suportados <header-set anchor-name="support-types" />

Todos os tipos primitivos do .NET são suportados, incluindo suas versões anuláveis: `Nullable<?>`.

* `string`
* `bool` ou `bool?`
* `decimal` ou `decimal?`
* `double` ou `double?`
* `int` ou `int?`
* `uint` ou `uint?`
* `DateTime` ou `DateTime?`
* `byte` ou `byte?`
* `short` ou `short?`
* `ushort` ou `ushort?`
* `long` ou `long?`
* `ulong` ou `ulong?`
* `float` ou `float?`
* `char` ou `char?`
* `Enum`/`Enum Flags` ou `Enum?`
* `Generic collections` (`IEnumerable`, `IList`, `ICollection`)
* `Arrays`

**Sintaxe genérica:**

```[action-name ][-|/|--][argument-name][=|:| ][value]```

**Sintaxe para `Boolean`:**

* Para o valor TRUE use: `true`, `1`, `+` (separado por espaço ou unido com o nome do argumento) ou omita o valor.
* Para o valor FALSE use: `false`, `0`, `-` (separado por espaço ou unido com o nome do argumento).

```
MyApp.exe -a  // true
MyApp.exe -a- // false
MyApp.exe -a+ // true
MyApp.exe -a - // false
MyApp.exe -a + // true
MyApp.exe -a true // true
MyApp.exe -a false // false
MyApp.exe -a 0 // true
MyApp.exe -a 1 // false
```

**Atribuições multiplas:**

Para argumentos que estão configurados com a `forma curta`, é possível definir o mesmo valor em diversos argumentos com apenas um traço `-`, veja:

```csharp
public void Main(char a, char b, char c) {};
```

```
MyApp.exe -abc  // true for a, b and c
MyApp.exe -abc- // false for a, b and c
MyApp.exe -abc+ // true for a, b and c
```

**Sintaxe para `Enums`:**

Os valores de entrada podem variar entre o nome do `Enum` no formato case-sensitive ou o seu número interno. Para `Enum Flags` utilize espaços para adicionar ao valor do argumento.

```csharp
[Flags]
public enum Verbose
{
    None = 0,
    All = 1,
    Info = 2,
    Success = 4,
    Critical = 8,
    Warning = 16,
    Error = 32,
    Quiet = 64
}

public void Main(Verbose verbose, string otherParameter = null);
```

```
MyApp.exe --verbose Error Info Success
MyApp.exe --verbose 32 2 Success
MyApp.exe Success EnumNotContainsThisString     // positional
```

No último exemplo, o valor "EnumNotContainsThisString" não pertence ao enum `Verbose`, sendo assim o próximo argumento receberá esse valor caso seu tipo seja compativél.

**Coleções genéricas e Arrays**

As listas/arrays tem o mesmo padrão de input, separe com um espaço para adicionar um novo item da lista. Caso seu texto tenha espaço em seu conteúdo, então adicione entre aspas no inicio e no fim de seu texto.

```csharp
public void Main(IEnumerable<decimal> myLst, string[] myArray = null);
```

```
MyApp.exe --my-lst 1.0 1.99
MyApp.exe 1.0 1.99 // positional
MyApp.exe --my-lst 1.0 1.99 --my-array str1 str2
MyApp.exe --my-lst 1.0 1.99 --my-array "string with spaces" "other string" uniqueWord
MyApp.exe 1.0 1.99 str1 str2 // positional
```

No último exemplo, o valor "str1" quebra a sequencia de números "1.0 1.99", sendo assim o próximo argumento receberá esse valor caso seu tipo seja compativél.

**Importante!**

Todos as conversões levam em consideração a cultura configurada na propriedade estática "CultureInfo.CurrentCulture".