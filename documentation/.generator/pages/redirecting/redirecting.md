# Redirecionamento de comandos <header-set anchor-name="redirectiong-commands" />

Para redirecionar a sua aplicação com uma nova sequência de comandos é muito simples, basta a sua action retornar uma instância da classe `RedirectResult` passando em seu construtor uma string contendo a nova sequência de comandos. Vale ressaltar que as instâncias dos comandos serão as mesmas, ou seja, o estado de cada comando não voltará ao inicio, apenas o fluxo de execução. Outro ponto importante é que qualquer ação depois da ação que retornou o `RedirectResult` não será mais chamado.

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

No exemplo abaixo a ação `Something` será executada porque ela esta definida antes do redirect.

```
C:\MyApp.exe something redirect-now my-value
Something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```

No exemplo abaixo a action `Something` não será executada porque ela esta definida depois do redirect.

```
C:\MyApp.exe redirect-now my-value something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```