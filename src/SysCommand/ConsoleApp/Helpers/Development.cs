using System;
using System.IO;

namespace SysCommand.ConsoleApp.Helpers
{
    public static class Development
    {
        private static string projectDirectory;
        private static object thisLock = new object();

        public static bool IsAttached
        {
            get
            {
                return System.Diagnostics.Debugger.IsAttached;
            }
        }

        public static string GetProjectDirectory()
        {
            lock (thisLock)
            {
                if (projectDirectory == null)
                {
                    var pathFull = FileHelper.GetCurrentDirectory<App>();

                    projectDirectory = pathFull;

                    // if TRUE is because in VisualStudio
                    var i = 1;
                    do
                    {
                        if (Directory.GetFiles(projectDirectory, "project.json").Length != 0)
                            return projectDirectory;
                        else if (Directory.GetFiles(projectDirectory, "*.csproj").Length != 0)
                            return projectDirectory;
                        else if (Directory.GetFiles(projectDirectory, "*.xproj").Length != 0)
                            return projectDirectory;
                        projectDirectory = GetHigherDirectoryPath(pathFull, i);
                        i++;
                    }
                    while (projectDirectory != null);
                    throw new System.Exception("No project files were found");
                }
            }

            return projectDirectory;
        }

        private static string GetHigherDirectoryPath(string srcPath, int upLevel)
        {
            string[] directoryElements = srcPath.Split(Path.DirectorySeparatorChar);
            if (upLevel >= directoryElements.Length)
            {
                return null;
            }
            else
            {
                string[] resultDirectoryElements = new string[directoryElements.Length - upLevel];
                for (int elementIndex = 0; elementIndex < resultDirectoryElements.Length; elementIndex++)
                {
                    resultDirectoryElements[elementIndex] = directoryElements[elementIndex];
                }
                return string.Join(Path.DirectorySeparatorChar.ToString(), resultDirectoryElements);
            }
        }
    }
}
