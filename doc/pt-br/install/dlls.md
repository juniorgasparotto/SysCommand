## DLLs do pacote !heading

  * SysCommand.dll: Contém toda a lógica de parse e execução de linhas de comandos. Pode ser utilizada em outros tipos de projetos como Web Application ou Windows Forms. Aqui NÃO deve existir nenhum uso a classe do .NET "System.Console".
  * SysCommand.ConsoleApp.dll: Contém diversos recursos que uma aplicação do tipo console application precisa. Tudo foi pensado para que o padrão MVC fosse o mais natural possível.
  * Dependencias `NewtonSoft.Json` e `System.Web.Razor`: São dependencias necessárias para ajudar em alguns recursos que serão explicados mais adiante na documentação.
