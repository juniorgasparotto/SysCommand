# Iniciando <header-set anchor-name="class-app" />

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instância da class `App` com suas possíveis customizações ou através do método estático `App.RunApplication` que disponibiliza um recurso chamado **simulador de console** que ajuda você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo.

A classe `App` esta no topo da hierarquia de classes, cada instância equivale a um contexto isolado que vai conter uma arvore de outros objetos que são exclusivos desse contexto. Nenhum recurso estático é usado aqui e isso é importante para ter a liberdade de criar quantas instâncias quiser em qualquer escopo.

Em seu construtor estão as primeiras configurações:

```csharp
public App(
           IEnumerable<Type> commandsTypes = null,
           bool enableMultiAction = true,
           bool addDefaultAppHandler = true,
           TextWriter output = null
       )
```

* `commandsTypes`: Especifica os tipos dos `Command` que serão utilizados em todo o processo. Caso seja `null` então o sistema buscará automáticamente qualquer classe que extenda de `Command`. Entenda melhor em <anchor-get name="specifying-commands" />.
* `enableMultiAction`: Liga ou desliga o comportamento de `MultiAction`. Por padrão, esse comportamento estará habilitado. Entenda melhor em <anchor-get name="using-the-multi-action-feature" />.
* `addDefaultAppHandler`: Caso seja `false` então NÃO cria o handler de eventos que é responsável pelo mecanismo padrão de `outputs` e controles de `erros` e dentre outros. O padrão é `true`. Entenda melhor em <anchor-get name="events" />.
* `output`: Redireciona a saída para o `TextWriter` especificado. Do contrário será usado por padrão o `Console.Out`.