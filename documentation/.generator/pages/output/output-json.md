## Saída como JSON <header-set anchor-name="output-json" />

O método `ViewJson()` serializa um modelo para JSON facilitando o transporte de informação.

**Exemplo:**

###### Commands/JsonCommand.cs

```csharp
public class JsonCommand : Command
{

    public string MyJson()
    {
        var list = new List<MyModel>
        {
            new MyModel() {Id = "1", Column2 = "Line 1 Line 1"},
            new MyModel() {Id = "2 " , Column2 = "Line 2 Line 2"},
            new MyModel() {Id = "3", Column2 = "Line 3 Line 3"}
        };

        return ViewJson(list);
    }

    public class MyModel
    {
        public string Id { get; set; }
        public string Column2 { get; set; }
    }
}
```

###### Tests

Input1:

```
MyApp.exe my-json
```

Outputs:

```
[
  {
    "Id": "1",
    "Column2": "Line 1 Line 1"
  },
  {
    "Id": "2 ",
    "Column2": "Line 2 Line 2"
  },
  {
    "Id": "3",
    "Column2": "Line 3 Line 3"
  }
]
```