# DLLs do pacote <header-set anchor-name="install-dlls" />

* `SysCommand.dll`: Contém toda a lógica de parse e execução de linhas de comandos. Tudo foi pensado para tornar o padrão MVC tão natural quanto possível.
* `NewtonSoft.Json` e `System.Web.Razor`: São dependências necessárias em alguns recursos que serão explicados na documentação.

## Passo a passo de como usar <header-set anchor-name="install-step-a-step" />

* Criar seu projeto do tipo `Console Application`
* Instale o `SysCommand` no seu projeto `Console Application`
* Na primeira linha do método `public int Program.Main(string[] args)`, adicione o código `return App.RunApplication()`.
* Criar uma classe, em qualquer lugar, que herde de `SysCommand.ConsoleApp.Command`.
* Criar suas propriedades com seus tipos `Nullable` e deixe-as como publicas. Elas se tornarão `arguments` no prompt de comando.
* Crie um método `Main()` sem parametros em sua classe para poder interceptar os inputs de suas propriedades. Utilize `Property != null` para identificar que a propriedade foi inserida.
* Crie métodos publicos, com ou sem parâmetros, para que eles se tornem `actions`. Caso tenha parâmetros opcionais, deixe-os configurados como `Nullable` pela mesma razão acima.
* Digite `help` no prompt de comando que será aberto para poder visualizar suas propriedades e métodos transformados em `arguments` e `actions`.
* Agora é só usar!