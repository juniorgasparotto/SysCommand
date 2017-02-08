# Iniciando <header-set anchor-name="class-app" />

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instância da class `App` com suas possíveis customizações ou atravez do método estático `App.RunApplication` que disponibiliza um recurso muito interressante de `simulação de console` ajudando você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo ou via "Command Line Arguments".

A classe `App` esta no topo da hierarquia de classes do sistema, cada instância é responsável por manter um contexto isolado da execução. Nenhum recurso estático é usado aqui e isso é importante para ter a liberdade de criar quantas instancias quiser em qualquer escopo.

Em seu construtor estão as primeiras configurações:

```csharp
public App(
           IEnumerable<Type> commandsTypes = null,
           bool enableMultiAction = true,
           bool addDefaultAppHandler = true
       )
```

* `commandsTypes`: Especifica os tipos dos `Command` que serão utilidados em todo o processo. Caso seja `null` então o sistema buscará automaticamente qualquer classe que extenda de `Command`. Entenda melhor em <anchor-get name="specifying-commands" />.
* `enableMultiAction`: Liga ou desliga o comportamento de `MultiAction`. Por padrão, esse comportamento estará ligado. Entenda melhor em <anchor-get name="using-the-multi-action-feature" />.
* `addDefaultAppHandler`: Caso seja `false` então NÃO cria o handler de eventos que é responsável pelo mecanismo padrão de `outputs` e controles de `erros` e dentre outros. O padrão é `true`. Entenda melhor em <anchor-get name="events" />.


