# Cancelamento da continuidade da execução <header-set anchor-name="stop-propagation" />

Quando existem muitas actions com o mesmo nome e assinatura, todas elas serão executadas juntas quando solicitada pelo usuário. Porém, você pode impedir isso usando o comando `ExecutionScope.StopPropagation()` dentro da sua action que você deseje que seja a última na pilha de execução.

**Exemplo:**

```csharp
public class StopPropagationCommand1 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        return "StopPropagationCommand1.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand1.StopPropagationAction2";
    }
}

public class StopPropagationCommand2 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        if (cancel)
        {
            ExecutionScope.StopPropagation();
        }

        return "StopPropagationCommand2.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand2.StopPropagationAction2";
    }
}

public class StopPropagationCommand3 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        return "StopPropagationCommand3.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand3.StopPropagationAction2";
    }
}
```

```
C:\MyApp.exe stop-propagation-action1
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
StopPropagationCommand3.StopPropagationAction1

C:\MyApp.exe stop-propagation-action1 --cancel
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
```

Perceba que ao utilizar o argumento "--cancel" a action "StopPropagationCommand3.StopPropagationAction1" não foi executada. Isso por que ela estava na última posição da pilha de execução e como a action "StopPropagationCommand2.StopPropagationAction1" cancelou a continuidade da execução, qualquer outra action da sequencia sera ignorada.

Outra possibilidade de uso do `StopPropagation` é quando existem multiplas actions no mesmo input. A lógica é a mesma, será cancelado todas as actions da pilha que estão depois da action que disparou o stop.

```
C:\MyApp.exe stop-propagation-action1 stop-propagation-action2
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
StopPropagationCommand3.StopPropagationAction1
StopPropagationCommand1.StopPropagationAction2
StopPropagationCommand2.StopPropagationAction2
StopPropagationCommand3.StopPropagationAction2

C:\MyApp.exe stop-propagation-action1 --cancel stop-propagation-action2
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
```
Perceba que a execução parou no mesmo ponto.

* Para desabilitar o recurso de multi-action, desative a propriedade `App.EnableMultiAction` antes do método `App.Run()`.

