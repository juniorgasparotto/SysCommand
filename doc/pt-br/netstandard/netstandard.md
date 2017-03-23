# Limitações do NETSTANDARD <header-set anchor-name="netstandard" />

* No netstandard temos a limitação de carregar apenas o assembly da execução e o proprio `SysCommand.dll`. Isso devido a falta da API `AppDomain`.
* Não temos a propriedade `this.App` disponível no construtor do `Command` devido a falta da API `FormatterServices.GetUninitializedObject`
* Os métodos `Command.GetActionMap()` e `Command.GetAction()` só estão disponíveis com as sobrecargas `Command.GetActionMap(Type[] paramTypes)` e `Command.GetAction(Type[] paramTypes)`. Use o método `SysCommand.Helpers.Reflection.T<...>()` para facilitar o uso, ele suporta até 10 inferências.
* Não temos a funcionalidade de template usando Razor. Será feito em breve.
* O nuget package ainda não suporta o arquivo "Program.cs.txt"

**Ficaremos atento com as próximas versões do netstandard, assim que essas APIs ficarem disponíves esses recursos também serão contemplados.**
