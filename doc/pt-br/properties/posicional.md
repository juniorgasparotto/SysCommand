## Habilitando o input posicional <header-set anchor-name="properties-positional" />

Para habilitar o input posicional basta ligar a flag `EnablePositionalArgs` em seu `Command`, contudo é importante validar o quanto isso necessário, pois muitos inputs posicionais podem complicar muito o uso da sua aplicação. Apesar do `SysCommand` estar bem preparado para esse tipo de input, não queremos que você polua o seu input.

```csharp
public class TestProperty3Command : Command
{
    public int? MyPosicionalProperty1 { get; set; }
    public int? MyPosicionalProperty2 { get; set; }

    public TestProperty3Command()
    {
        this.EnablePositionalArgs = true;
    }

    public void Main()
    {
        if (MyPosicionalProperty1 != null)
            App.Console.Write("MyPosicionalProperty1=" + MyPosicionalProperty1);
        if (MyPosicionalProperty2 != null)
            App.Console.Write("MyPosicionalProperty2=" + MyPosicionalProperty2);
    }
}
```

```
C:\MyApp.exe --my-posicional-property1 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2
```

Você pode também controlar a posição de cada propriedade dentro do input usando a configuração `Position`.

**Exemplo:**

```csharp
public class TestProperty3Command : Command
{
    [Argument(Position = 2)]
    public int? MyPosicionalProperty1 { get; set; }

    [Argument(Position = 1)]
    public int? MyPosicionalProperty2 { get; set; }

    public TestProperty3Command()
    {
        this.EnablePositionalArgs = true;
    }

    public void Main()
    {
        if (MyPosicionalProperty1 != null)
            App.Console.Write("MyPosicionalProperty1=" + MyPosicionalProperty1);
        if (MyPosicionalProperty2 != null)
            App.Console.Write("MyPosicionalProperty2=" + MyPosicionalProperty2);
    }
}
```

```
C:\MyApp.exe --my-posicional-property1 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 2
MyPosicionalProperty1=2
MyPosicionalProperty2=1
```