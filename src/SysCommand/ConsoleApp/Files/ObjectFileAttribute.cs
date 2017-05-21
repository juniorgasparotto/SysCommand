using System;

namespace SysCommand.ConsoleApp.Files
{
    /// <summary>
    /// This attribute is useful for attaching a file name in a given class. 
    /// So, when using the methods Save<T>(T obj) , Get<T>() Remove<T>() or, GetOrCreate<T>() 
    /// the name of the type of the object will no longer be used. 
    /// The name set on the property ObjectFile(FileName="file.json") will always be used for this type.
    /// </summary>
    public class ObjectFileAttribute : Attribute
    {
        /// <summary>
        /// Location to save/get
        /// </summary>
        public string FileName { get; set; }
    }
}
