## Classe `App` !heading

A classe `App` é a principal classe do sistema, ela é responsável por manter um contexto isolado por cada instancia `App`. Nenhum recurso estático é usado aqui, isso é importante para você ter a liberdade de criar quantas instancias quiser em qualquer escopo.

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instancia da class `App` com suas possíveis customizações ou atravez do método estático `App.RunApplication` que disponibiliza um recurso muito interressante de `simulação de console` ajudando você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo ou via "Command Line Arguments".

Em seu construtor estão as configurações mais importantes, são eles:

```csharp
public App(
           IEnumerable<Type> commandsTypes = null,
           bool enableMultiAction = true,
           bool addDefaultAppHandler = true
       )
```

* `commandsTypes`: Especifica os tipos dos `Command` que serão utilidados em todo o processo. Caso seja `null` então o sistema buscará automaticamente qualquer classe que extenda de `Command`. Entenda melhor em `Especificando os tipos de comandos`.
* `enableMultiAction`: Liga ou desliga o comportamento de `MultiAction`. Por padrão, esse comportamento estará ligado. Entenda melhor em `Utilizando o recurso de MultiAction`
* `addDefaultAppHandler`: Caso seja `false` então NÃO cria o handler de eventos que é responsável pelo mecanismo padrão de `outputs` e controles de `erros` e dentre outros. O padrão é `true`. Entenda melhor em `Controle de eventos`
