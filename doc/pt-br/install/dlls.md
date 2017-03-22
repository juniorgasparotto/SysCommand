# DLLs do pacote <header-set anchor-name="install-dlls" />

  * `SysCommand.dll`: Contém toda a lógica de parse e execução de linhas de comandos. Tudo foi pensado para que o padrão MVC fosse o mais natural possível.
  * Dependencias `NewtonSoft.Json` e `System.Web.Razor`: São dependencias necessárias para ajudar em alguns recursos que serão explicados mais adiante na documentação.

## Passo a passo da instalação <header-set anchor-name="install-step-a-step" />

* Criar seu projeto do tipo `Console Application`
* Instale o `SysCommand` no seu projeto `Console Application`
* Na primeira linha de seu método `public int Program.Main(string[] args)` adicione o código `return App.RunApplication()`.
* Criar uma classe, em qualquer lugar, que herde de `SysCommand.ConsoleApp.Command`.
* Criar suas propriedades com seus tipos `Nullable` e deixe-as como publicas. Elas se tornarão `arguments` no prompt de comando.
* Crie um método `Main()` sem parametros em sua classe para poder interceptar os inputs de suas propriedades. Utilize `Property != null` para identificar que a propriedade foi inputada.
* Crie métodos publicos, com ou sem parâmetros, para que eles se tornem `actions`. Caso tenha parâmetros optionais deixe-os como `Nullable` pela mesma razão acima.
* Digite `help` no prompt de comando que abrirá para poder visualizar suas propriedades e métodos convertidos em `arguments` e `actions`.
* Agora é só usar!





