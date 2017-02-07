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

**Sintaxe para `string`:**

As `strings` podem ser utilizadas de duas formas:

* Um texto com espaços: Utilize aspas `"..."` para determinar o valor de um argumento string que contém espaços em seu conteúdo, do contrário você terá um erro de parse.
* Um texto sem espaços: Não é obrigatório o uso de aspas, basta inserir seu valor diretamente.

```
MyApp.exe --my-string oneWord
MyApp.exe --my-string "oneWord"
MyApp.exe --my-string "two words"
```

**Sintaxe para `char`:**

Assim como em .NET os chars podem ter valores com apenas um caracter ou com um número que represente seu valor na escala de caracteres.]

```
MyApp.exe --my-char 1
MyApp.exe --my-char A
```

**Sintaxe para `int`, `long`, `short` e suas variações "u" :**

São entradas númericas onde a única regra é o valor inserido não ultrapassar o limite de cada tipo.

```
MyApp.exe --my-number 1
MyApp.exe --my-number 2
MyApp.exe --my-number 999999
```

**Sintaxe para `decimal`, `double` e `float`:**

Para esses tipos é possível utilizar números inteiros ou números decimais. Só fique atento para a configuração de cultura da sua aplicação. Se for `pt-br` utilize o separador `,`; Para o formato americano utilize `.`;

_EN-US:_

```
MyApp.exe --my-number 10
MyApp.exe --my-number 0.99
```

_PT-BR:_

```
MyApp.exe --my-number 10
MyApp.exe --my-number 0,99
```

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

**Sintaxe para `DateTime`:**

Assim como os números decimais, o formato de data suportado depende da cultura que estiver configurado em sua aplicação.

_EN-US:_

```
MyApp.exe --my-date "12/13/2000 00:00:00"
```

_PT-BR:_

```
MyApp.exe --my-date "13/12/2000 00:00:00"
```

_UNIVERSAL:_

```
MyApp.exe --my-date "2000-12-13 00:00:00"
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