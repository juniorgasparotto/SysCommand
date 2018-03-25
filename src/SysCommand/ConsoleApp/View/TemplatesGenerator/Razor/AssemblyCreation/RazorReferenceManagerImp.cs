//using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class RazorReferenceManagerImp  //: RazorReferenceManager
    {
        private Dictionary<string, AssemblyMetadata> _cache = new Dictionary<string, AssemblyMetadata>();
        private static List<string> ValidExtensions = new List<string>() { ".dll", ".exe" };

        private readonly IEnumerable<Assembly> assemblies;
        private object _compilationReferencesLock = new object();
        private bool _compilationReferencesInitialized;
        private IReadOnlyList<MetadataReference> _compilationReferences;

        public IReadOnlyList<MetadataReference> CompilationReferences
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _compilationReferences,
                    ref _compilationReferencesInitialized,
                    ref _compilationReferencesLock,
                    GetCompilationReferences);
            }
        }

        public RazorReferenceManagerImp(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }

        private IReadOnlyList<MetadataReference> GetCompilationReferences()
        {
            var compilationReferences = new List<MetadataReference>();
            foreach (var a in assemblies)
                compilationReferences.Add(GetMetadataReference(a.Location));
            return compilationReferences;
        }

        private MetadataReference GetMetadataReference(string assetPath)
        {
            var extension = Path.GetExtension(assetPath);

            string path = assetPath;
            if (string.IsNullOrEmpty(extension) || !ValidExtensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var ext in ValidExtensions)
                {
                    path = assetPath + ext;
                    if (File.Exists(path))
                    {
                        break;
                    }
                }
            }

            if (!_cache.TryGetValue(path, out AssemblyMetadata assemblyMetadata))
            {
                if (File.Exists(path))
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                        assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                        _cache[path] = assemblyMetadata;
                    }
                }
            }

            return assemblyMetadata?.GetReference();
        }
    }
}
