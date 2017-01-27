##Redirecionamento de comandos

Para redirecionar a sua aplicação com uma nova sequencia de comandos é muito simples, basta a sua action retornar uma instancia da classe `RedirectResult` passando em seu construtor uma string contendo a nova sequencia de comandos. Vale ressaltar que as instancias dos comandos serão as mesmas, ou seja, o estado de cada comando não voltará ao inicio, apenas o fluxo de execução. Outro ponto importante é que qualquer input depois dessa action não será chamado, ou seja, a execução reinicia com o novo comando no momento em que existe um retorno do tipo `RedirectResult`.

**Exemplo:**

```csharp
public class RedirectCommand : Command
{
    private int _count;

    public RedirectResult RedirectNow(string arg)
    {
        _count++;
        App.Console.Write($"Redirecting now!!. Count: {_count}");
        return new RedirectResult("redirected", "--arg", arg);
    }

    public string Something()
    {
        return "Something";
    }
    
    public string Redirected(string arg)
    {
        _count++;
        return $"Redirected: {arg}. Count: {_count}";
    }
}
```

No exemplo abaixo a action `Something` será executada, pois esta antes do redirect.

```
C:\MyApp.exe something redirect-now my-value
Something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```

No exemplo abaixo a action `Something` não será executada, pois esta depois do redirect.

```
C:\MyApp.exe redirect-now my-value something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```

* Para desabilitar o recurso de multi-action, desative a propriedade `App.EnableMultiAction` antes do método `App.Run()`.