##Output tabelado

A classe `SysCommand.ConsoleApp.View.TableView` tras o recurso de `output tabelado` que pode ser muito útil para apresentar informações de forma rápida e visualmente mais organizada. É claro que tudo depende da quantidade de informação que você quer exibir, quanto maior, pior a visualização.

**Exemplo:**

######Commands/TableCommand.cs

```csharp
public class TableCommand : Command
{
    public string MyTable()
    {
        var list = new List<MyModel>
        {
            new MyModel() {Id = "1", Column2 = "Line 1 Line 1"},
            new MyModel() {Id = "2 " , Column2 = "Line 2 Line 2"},
            new MyModel() {Id = "3", Column2 = "Line 3 Line 3"}
        };

        return TableView.ToTableView(list)
                        .Build()
                        .ToString();
    }

    public class MyModel
    {
        public string Id { get; set; }
        public string Column2 { get; set; }
    }
}
```
######Tests

Input1: ```MyApp.exe my-table```

Outputs:

```
Id   | Column2
--------------------
1    | Line 1 Line 1
--------------------
2    | Line 2 Line 2
--------------------
3    | Line 3 Line 3
--------------------
```