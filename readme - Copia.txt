Funcionalidade b�sica:

Ao criar-se uma classe de comando de a��o (que herda de CommandAction), em qualquer lugar do seu projeto console application, todos os seus m�todos publicos (e seus para�metros) ser�o habilitados para serem chamados via prompt de comando automaticamente. Contudo, � poss�vel customizar esse processo da seguinte forma:

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
- M�todos publicos com o nome "Default" ou com o atributo "Action(IsDefault=true)" podem ser acessados via prompt sem a necessidade do nome da a��o, ou seja, passando apenas os argumentos.

class HelloWordCommand : CommandAction
{
	public HelloWordCommand() {
		this.AddPrefixInActions = true;
		// this.PrefixActions = "custom-prefix";
	}
	
	public void Default(string p1) {
		
	}
	
	[Action(IsDefault=true)]
	public void OtherDefault(string p1) {
		
	}
	
	[Action(Ignore=true)]
	public void IsIgnored(string p1, string p2, int p) {
		
	}
	
	public void FirstMethod(string p1, string p2, int p) {
		
	}
	
	[Action(Name="action-with-other-name", AddPrefix = false)]
	public void SecondMethod
	(
		[Argument(ShortName="p")] string p1,
								  string p2, 
		[Argument(LongName="parameter")] int p
	)
	{
		
	}
}

$~: MyApp.exe default [arguments...]									-> Default(string p1)
$~: MyApp.exe other-default [arguments...]								-> OtherDefault(string p1)
$~: MyApp.exe --p1 "test"												-> Default(string p1)
#~: MyApp.exe hello-word-first-method --p1 "..." --p2 "..." -p 0		-> FirstMethod(string p1, string p2, int p)
#~: MyApp.exe hello-word-first-method :p1 "..." :p2 "..." :p 0			-> FirstMethod(string p1, string p2, int p)
#~: MyApp.exe action-with-other-name --p1 "..." --p2 "..." -p 0			-> SecondMethod(string p1, string p2, int p)

Controle de sele��o e execu��o de a��es:

O controle � feito em duas etapas, "sele��o" e "execu��o". Esse controle � feito exclusivamente para executar apenas um m�todo por command e assim conseguir escolher o melhor m�todo em caso de overloads de m�todos ou tamb�m em mais de um m�todo default no mesmo command. Os passos s�o:
	
	1) Ser�o escolhidos os m�todos que tiverem as mesmas quantidades de argumentos do que foi enviado no prompt.
	2) Dos m�todos escolhidos:
		2.1) Ser�o selecionados, como canditados a execu��o, os m�todos que tiverem a maior quantidade de nomes de argumentos que combinaram com os nomes dos argumentos que foram enviado.
		2.2) Em caso de haver dois ou mais canditados de execu��o, o m�todo executado ser� o que foi definido primeiro.
	3) Em caso de haver dois m�todos iguais em commands diferentes, ambos ser�o executados respeitando a ordem de cria��o dos commands ou a propriedade "Command.OrderExecution".
	
class HelloWordCommand : CommandAction
{
	public void save(string a) {}
	public void save(int a) {}
	public void save(int a, string b) {}
	public void save(int a, string c) {}
}

class OtherCommand : CommandAction
{
	public void OtherCommand() {
		this.OrderExecution = -1;
	}
	
	public void save(string a) {}
	public void save(int a) {}
	public void save(int a, string b) {}
	public void save(int a, string c) {}
}

Cen�rio 2.1:

$~: MyApp.exe -a "1" -b "..."

Escolhidos: Ambos os m�todos tem a mesma quantidade de argumentos do que foi enviado.	
	OtherCommand.save(int a, string b);
	OtherCommand.save(int a, string c);
	HelloWordCommand.save(int a, string b);
	HelloWordCommand.save(int a, string c);
Executados: Ser� executado o m�todo que der match com a maior quantidade de argumentos enviados.
	OtherCommand.save(int a, string b);
	HelloWordCommand.save(int a, string b);
	
Cen�rio 2.2:

$~: MyApp.exe -a "1"

Escolhidos: Ambos os m�todos tem a mesma quantidade de argumentos que foi enviado.
	OtherCommand.save(string a);
	OtherCommand.save(int a);
	HelloWordCommand.save(string a);
	HelloWordCommand.save(int a);
Executados: Ser� executado o primeiro m�todo, pois ele foi definido primeiro.
	OtherCommand.save(string a);
	HelloWordCommand.save(string a);
		
Multiplas a��es:

Existe a poss�bilidade de em um mesmo input, no prompt de comando, ser executado mais de uma a��o. Por�m, para que isso seja poss�vel � necess�rio configurar a propriedade "App.Current.ActionCharPrefix='[algum-char]'" antes do m�todo "App.Current.Run()". Com essa propriedade configurada o comando abaixo teria o seguinte comportamento:

class Program : App
{
	static int Main(string[] args)
	{   
		while (true)
		{
			App.Initialize();
			App.Current.ActionCharPrefix = '$';
			App.Current.Run();
			if (!App.Current.InDebug)
				return App.Current.Response.Code;
		}
	}
}

class HelloWordCommand : CommandAction
{
	public void save(int id) {}
	public void delete(int id) {}
	public void Default(string value) {}
}

$~: MyApp.exe $save --id 1 $delete --id 1 $default --value \$UtilizarBarraParaEscaparOCharSelecionadoDaAction
	HelloWordCommand.save(int id);
	HelloWordCommand.delete(int id);
	HelloWordCommand.default(string value);
		
Argumentos posicionais:

Ao tentar utilizar argumentos posicionais, ser� feito uma analise interna para tentar organizar as posi��es enviadas com as posi��es de cada m�todo/a��o e o m�todo ser� executado em caso de sucesso.

Para multiplas a��es:

$~: MyApp.exe $save 1 $delete 1 $default \$UtilizarBarraParaEscaparOCharSelecionadoDaAction
				  [id]        [id]     [value]
				  
Para a��es simples:

$~: MyApp.exe default save
				     [value]
				  
Para a��es default: � necess�rio escapar, pois existe uma a��o com o nome "save" e com o escape ser� poss�vel considerar esse input como valor e n�o como uma a��o.

$~: MyApp.exe \save
			 [value]

--------------------------------------------

Outras funcionalidades:

	- � poss�vel criar commands que funcionam apenas em modo de debug no visual studio, isso � �til para criar a��es de helper durante o debug como no caso do command nativo "ClearCommand" que limpa o console de debug quando enviado o argumento "--clear". Para isso utilize no construtor do command "OnlyInDebug=true".
	- � poss�vel ignorar um command, mas isso tem que ocorrer antes da linha "App.Run()". Para isso utilize "App.Current.IgnoreCommand<ClearCommand>()".
	- � poss�vel cancelar a continua��o da execu��o de outras a��es usando o m�todo "App.Current.StopPropagation()"
	- Sugerimos utilizar o m�todo "App.Current.Response(message, force)" para exibir informa��es no console. Ele esta fortemente ligado aos parametros "--verbose, -v" e "--quiet" do command nativo "VerboseCommand". O "verbose" pode determinar n�veis do tipo de mensagem que pode ou n�o ser exibido e o "quiet" sobrep�e o "verbose" n�o exibindo nenhuma informa��o, mesmo quando "force=true".
		--verbose all,none,info,success,warning,critical,error,question
		--quiet true|false|0|1
	- Por default o command nativo "HelperCommand" d� a poss�bilidade de exibir ajuda utilizando o argumento "--help, -?"
	
--------------------------------------------
N�o � poss�vel coexistir a��es padr�o com a��es nomeadas, isso ocorre para evitar a situa��o
	save --p1 "a" -> save(string p1)
	--p1 "a"	  -> default(string p1)