# Gerênciador de objetos em forma de arquivos <header-set anchor-name="file-manager" />

Esse recurso é muito útil para persistir informações em arquivo no formato `Json`. Ele utiliza a dependência do framework `NewtonSoft.Json` para fazer todo o trabalho de serialização e deserialização.

A classe `SysCommand.ConsoleApp.Files.JsonFileManager` é a responsável pelo controle padrão de gerência de objetos em forma de arquivos. Nela contém alguns recursos que vão te ajudar a ganhar tempo caso precise persistir objetos. O formato será sempre `Json`.

_Propriedades:_

* `DefaultFolder`: Nome da pasta padrão. O padrão é `.app`.
* `bool SaveInRootFolderWhenIsDebug`: Determina se a pasta padrão será criada na raiz do projeto quando esta em modo de debug dentro do Visual Studio. Isso ajuda a visualizar os arquivos gerados usando a opção `Show all files` do `Solution Explorer`.
* `string DefaultFilePrefix`: Adiciona um prefixo em todos os arquivos. O padrão é `null`.
* `string DefaultFileExtension`: Especifica a extensão dos arquivos. O padrão é `.json`.
* `bool UseTypeFullName`: Determina se a formatação dos tipos `T` contemplará o nome completo do tipo. O padrão é `false`.

_Métodos:_

* `Save<T>(T obj)`: Salva um objeto na pasta padrão onde o nome do arquivo será o nome tipo `T` formatado, com exceção de classes que tem o atributo `ObjectFile`.
* `Save(object obj, string fileName)`: Salva um objeto na pasta padrão com um nome especifico.
* `Remove<T>()`: Remove um objeto na pasta padrão onde o nome do arquivo será o nome tipo `T` formatado, com exceção de classes que tem o atributo `ObjectFile`.
* `Remove(string fileName)`: Remove um objeto na pasta padrão com um nome especifico.
* `T Get<T>(string fileName = null, bool refresh = false)`: Retorna um objeto da pasta padrão.
  * `fileName`: Indica o nome do arquivo, caso seja `null` o nome do tipo `T` será usado na busca, com exceção de classes que tem o atributo `ObjectFile`.
  * `refresh`: Se `false` buscará no cache interno caso já tenha sido carregado anteriormente. Do contrário será forçado o carregamento do arquivo.
* `T GetOrCreate<T>(string fileName = null, bool refresh = false)`: Mesmo comportamento do método acima, porém cria uma nova instância quando não encontrar o arquivo na pasta padrão. É importância dizer que o arquivo não será criado, apenas a instância do tipo `T`. Para salvar fisicamente é necessário utilizar o método `Save`.
* `string GetObjectFileName(Type type)`: Retorna o nome do tipo formatado ou se estiver usando o atributo `ObjectFile`, retorna o valor da propriedade `FileName`.
* `string GetFilePath(string fileName)`: Retorna o caminho do arquivo dentro da pasta padrão.

_Atributo `ObjectFile`:_

Esse atributo é útil para fixar um nome de arquivo em uma determinada classe. Assim, ao usar os métodos `Save<T>(T obj)`, `Get<T>()`, `Remove<T>()` ou `GetOrCreate<T>()` o nome do tipo do objeto não será mais utilizado. O nome fixado na propriedade `ObjectFile(FileName="file.json")` será sempre usado para esse tipo.

**Exemplo:**

```csharp
namespace Example.FileManager
{
    using SysCommand.ConsoleApp;
    using SysCommand.ConsoleApp.Files;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command1 : Command
    {
        private JsonFileManager fileManager;

        public Command1()
        {
            fileManager = App.Items.GetOrCreate<JsonFileManager>();
        }

        public void Save(string title, string description = null)
        {
            var tasks = fileManager.GetOrCreate<Tasks>();
            tasks.LastUpdate = DateTime.Now;

            var task = tasks.AllTasks.FirstOrDefault(t => t.Title == title);
            if (task == null)
            {
                task = new Task
                {
                    Id = tasks.AllTasks.Count + 1,
                    Title = title,
                    Description = description,
                    DateAndTime = DateTime.Now
                };
                tasks.AllTasks.Add(task);
            }

            fileManager.Save(tasks);
        }

        public void Get(string title)
        {
            var tasks = fileManager.GetOrCreate<Tasks>();
            this.ShowTask(tasks.AllTasks.Where(t => t.Title.Contains(title)));
        }

        private void ShowTask(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                this.ShowTask(task);
        }

        private void ShowTask(Task task)
        {
            if (task == null)
            {
                App.Console.Error("Task not found");
                return;
            }

            App.Console.Write("Id: " + task.Id);
            App.Console.Write("Title: " + task.Title ?? "-");
            App.Console.Write("Description: " + task.Description ?? "-");
            App.Console.Write("Date: " + task.DateAndTime);
        }

        [ObjectFile(FileName = "tasks")]
        public class Tasks
        {
            public DateTime LastUpdate { get; set; }
            public List<Task> AllTasks { get; set; } = new List<Task>();
        }

        public class Task
        {
            public int Id { get; set; }
            public DateTime DateAndTime { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }
    }
}
```

```
MyApp.exe save "title1" "description1"
MyApp.exe save "title2" "description2"
MyApp.exe get "title"
Id: 1
Title: title1
Description: description1
Date: 20/02/2017 21:22:19
Id: 2
Title: title2
Description:
Date: 20/02/2017 21:24:20
```

Note que para criar uma instância de `JsonFileManager` foi utilizado o escopo do contexto `App.Items`, isso é útil para manter apenas uma instância desse gerenciador, economizando memória e mantendo as mesmas configurações em qualquer lugar que for utiliza-lo. É claro que se as configurações forem especificas, então será necessário criar uma nova instância com outras configurações no escopo que achar melhor.