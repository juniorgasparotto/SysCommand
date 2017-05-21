using System.IO;
using System.Reflection;

namespace SysCommand.ConsoleApp.Helpers
{
    /// <summary>
    /// Helper to working with files
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Get current directory
        /// </summary>
        /// <returns>Current directory</returns>
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>True if exists</returns>
        public static bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        /// <summary>
        /// Get content from file
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>File content</returns>
        public static string GetContentFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            return File.ReadAllText(fileName);
        }
        
        /// <summary>
        /// Remove file if exists
        /// </summary>
        /// <param name="fileName">File path</param>
        public static void RemoveFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Save content in file
        /// </summary>
        /// <param name="content">Content to save</param>
        /// <param name="fileName">File location</param>
        public static void SaveContentToFile(string content, string fileName)
        {
            CreateFolderIfNeeded(fileName);
            File.WriteAllText(fileName, content);
        }

        /// <summary>
        /// Create the folder if not existing for a full file name
        /// </summary>
        /// <param name="filename">full path of the file</param>
        public static void CreateFolderIfNeeded(string filename)
        {
            string folder = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static string GetUniversalFileName(string fileName)
        {
            // fix to unix
            return fileName.Replace("\\", "/").Replace("//", "/");
        }

    }
}
