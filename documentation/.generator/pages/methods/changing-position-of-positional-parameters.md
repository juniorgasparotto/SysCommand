## Trocando a posição de parâmetros posicionais <header-set anchor-name="methods-changing-position" />

A propriedade `ArgumentAttribute(Position=X)` também funciona para parâmetros da mesma forma que funciona para propriedades. Não é um recurso que faça muito sentido, mas é importante documenta-lo.

**Exemplo:**

```csharp
public class Method5Command : Command
{
    public string MyActionWithArgsInverted(
        [Argument(Position = 2)]
        string arg0,
        [Argument(Position = 1)]
        string arg1
    )
    {
        return "MyActionWithArgsInverted";
    }
}
```

```
C:\MyApp.exe my-action-with-args-inverted 1 2
arg0 = '2'; arg1 = '1'
```