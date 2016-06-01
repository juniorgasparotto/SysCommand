Funcionalidade b�sica:

Ao criar-se uma classe de comando de a��o (que herda de CommandAction) todos os seus m�todos publicos (e seus para�metros) ser�o habilitados para serem chamados via prompt de comando. Contudo, � poss�vel customizar esse processo da seguinte forma:

- O m�todo, mesmo sendo publico, pode ser ignorado  usando o atributo "Action(Ignore=true)".
- O padr�o usado no prompt de comando ser� o nome do m�todo em modo de lowercase, por�m as letras maiusculas contidas no meio do m�todo ter�o um "-" as antecedendo.
- O padr�o pode ser sobrescrito pelo atributo 'Action(Name="...")'
- O nome do m�todo pode ser alterado caso o "command" use prefixo em seu construtor (AddPrefixInActions). Isso afeta todas as a��es, ou seja, ser� adicionado um prefixo antes do nome da a��o. 
	Esse prefixo ser� o nome da classe convertida em lowercase e removendo o sufixo "Command" caso ele exista no final do nome da classe.
	Esse prefixo pode ser alterado caso a propriedade "PrefixActions" seja especificada.
	Esse prefixo ser� ignorado quando o atributo "Action(AddPrefix = false)" for especificado
	
- Todos os argumentos dos m�todos seguir�o o formato especificado pelo framework "FluentCommandLineParser": '-', '/', ':', '='.
	"-": Para argumentos de apenas um caracter (shortName)
	"--": Para argumentos de mais de um caracter (longName)
	"=", ":", "/": Para argumentos de mais de um caracter (longName)
- Argumentos com apenas um caracter ser�o convertidos autom�ticamente para "shortName", ou seja, ser� acessado apenas por um tra�o "-".
- Argumentos podem ser customizados usando o atributo "Argument(ShortName="...", LongName="...",Help="...", ShowHelpComplement=true|false)"
	- ShortName: Cria/Altera uma entrada no prompt para argumentos de apenas um caractere
	- LongName: Cria/Altera uma entrada no prompt para argumentos de mais de um caractere
	- Help: Cria um help para o argumento
	- ShowHelpComplement: Exibe (true) ou n�o (false) o label "Is required" para argumentos sem default value ou "Is optional (Default: '...'" para argumentos com valor default. O default � true.
	- Existem outras propriedades nesse atributo, por�m elas n�o s�o suportadas para argumentos de m�todos/a��es.
- M�todos publicos com o nome "Default" ou com o atributo "Action(IsDefault=true)" podem ser acessados sem a especifica��o do nome da a��o, ou seja, passando direto os argumentos.

- Em uma requisi��o, apenas um m�todo/a��o pode ser escolhido para cada command.
- Em caso de sobrecarga de m�todo/a��o no mesmo "command" ganhar� o m�todo com a maior proximidade de argumentos com base no que foi enviado. 
- Em caso de haver dois ou mais "commands" com o mesmo nome de a��o, ambos ser�o escolhidos e chamados.

- Em caso de haver dois ou mais m�todos default no mesmo "command" ganhar� o m�todo que tem menos argumentos.

class HelloWordCommand : CommandAction
{
	public HelloWordCommand() {
		this.AddPrefixInActions = true;
		// this.PrefixActions = "custom-prefix";
	}
	
	public void Default(string p1) {
		...
	}
	
	[Action(IsDefault=true)]
	public void OtherDefault(string p1) {
		...
	}
	
	[Action(Ignore=true)]
	public void IsIgnored(string p1, string p2, int p) {
		...
	}
	
	public void FirstMethod(string p1, string p2, int p) {
		...
	}
	
	[Action(Name="action-with-other-name", AddPrefix = false)]
	public void FirstMethod2
	(
		[Argument(ShortName="p")] string p1,
								  string p2, 
		[Argument(LongName="parameter")] int p
	)
	{
		...
	}
}

$~: MyApp.exe default [arguments...]
$~: MyApp.exe [arguments...]
$~: MyApp.exe other-default [arguments...]
#~: MyApp.exe hello-word-first-method --p1 "..." --p2 "..." -p 0
#~: MyApp.exe hello-word-first-method :p1 "..." :p2 "..." :p 0
#~: MyApp.exe action-with-other-name --p1 "..." --p2 "..." -p 0

default()
default(string a)
default(string a = null)
save(string a = null)
save(int a = 0)
save()


[nada] 
	default()
	
save
	save()
	save(int a = 0)
	default(string a)
	
N�o � poss�vel coexistir a��es padr�o com a��es nomeadas, isso ocorre para evitar a situa��o
	save --p1 "a" -> save(string p1)
	--p1 "a"	  -> default(string p1)