# Tipos de comandos <header-set anchor-name="kind-of-commands" />

Atualmente existem tres tipos de comandos:

**Comandos de usuário**

São os comandos comuns e que herdam apenas da class `Command`. 

**Comandos de help**

São os comandos que herdam da classe `Command` e implementam a interface `IHelpCommand`. Contudo, apenas um será utilizado. 

**Comandos de debug**

Os comandos de debug são comandos que são carregados apenas durante o debugging do Visual Studio. Um bom exemplo seria o comando interno "ClearCommand", ele disponibiliza a action "clear" para limpar o prompt de comando que o Visual Studio abre durante o processo de debug. Para criar um comando de debug basta habilitar a flag `Command.OnlyInDebug`.

```csharp
public class ClearCommand : Command
{
    public ClearCommand()
    {
        this.HelpText = "Clear window. Only in debug";
        this.OnlyInDebug = true;
    }

    public void Clear()
    {
        Console.Clear();
    }
}
```