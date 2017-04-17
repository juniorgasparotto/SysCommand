## Parametros optionais <header-set anchor-name="methods-optional-params" />

Os parametros opcionais são uteis para evitar a criação de sobrecargas e no caso de uma aplicação console ajuda a criar `actions` com diversas opções, mas não obrigando o usuário a preencher todas.

Por segurança, ao usar parametros opcionais, obte por utilizar todos os tipos primitivos como `Nullable` para _garantir que o usuário fez o input_. Ou utilize o método `GetAction()` para verificar se o parametro foi mapeado, ou seja, se teve algum tipo de input.

**Exemplo:**

```csharp
public class Method1Command : Command
{
    public string MyAction2(int? arg0 = null, int arg1 = 0)
    {
        // unsafe, because the user can enter with value "--arg1 0" and you never know.
        if (arg1 != 0)
            App.Console.Write("arg1 wrong way to do it!");

        // safe, but bureaucratic
        if (this.GetAction().Arguments.Any(f => f.Name == "arg1" && f.IsMapped))
            App.Console.Write("arg1 has input");

        // recommended. the best way! 
        if (arg0 != null)
            App.Console.Write("arg0 has input");

        return "MyAction2";
    }
}
```

```
C:\MyApp.exe my-action2
MyAction2

C:\MyApp.exe my-action2 --arg0 99
arg0 has input
MyAction2

C:\MyApp.exe my-action2 --arg1 0
arg1 has input
MyAction2
```

Observação: Não utilize o método `GetAction()` em métodos que não são `actions`, você terá uma exception.