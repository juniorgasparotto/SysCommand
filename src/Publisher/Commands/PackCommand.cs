using Publisher.Core;
using SysCommand.ConsoleApp;
using System;
using System.IO;

namespace Publisher.CommandSpecific
{
    public class PackCommand : Command
    {
        public void Pack()
        {
            var configs = AppInfo.GetAppInfo().Configurations;
            foreach (var c in configs)
                Pack(c.Name);
        }

        public void Pack(string config)
        {
            try
            {
                App.Console.Write($"Start packing '{config}': {DateTime.Now}");
                
                #region 2) Clear unused files
                ClearUnused(config);
                #endregion

                #region 3) Zip
                Zip(config);
                #endregion
            }
            catch (Exception ex)
            {
                App.Console.Error(ex.Message);
            }
            finally
            {
                App.Console.Write($"End: {DateTime.Now}");
            }
        }

        #region

        private void ClearUnused(string config)
        {
            var listUnused = new string[]
            {
                "MarkdownGenerator.application",
                "MarkdownGenerator.exe.manifest",
                "MarkdownGenerator.pdb",
                "HtmlAgilityPack.pdb",
                "HtmlAgilityPack.xml",
                "Newtonsoft.Json.xml"
            };

            foreach (var name in listUnused)
            {
                var file = Path.Combine(PathCommand.BuildDirectory, config, name);
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        private void Zip(string config)
        {
            var appInfo = AppInfo.GetAppInfo();
            var configuration = appInfo.GetConfiguration(config, true);

            var source = Path.Combine(PathCommand.BuildDirectory, config);
            var destination = Path.Combine(PathCommand.GitHubPackDirectory, configuration.PackName);

            Utils.CreateFolderIfNeeded(destination);

            if (File.Exists(destination))
                File.Delete(destination);

            System.IO.Compression.ZipFile.CreateFromDirectory(source, destination);
        }

        #endregion
    }
}
