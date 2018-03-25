using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyModel;
using DependencyContextCompilationOptions = Microsoft.Extensions.DependencyModel.CompilationOptions;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal partial class CSharpCompiler
    {
        private readonly RazorReferenceManagerImp _referenceManager;
        private readonly string _appName;
        private readonly bool _isDevelopment;
        private readonly DebugInformationFormat _pdbFormat = SymbolsUtility.SupportsFullPdbGeneration() ?
            DebugInformationFormat.Pdb :
            DebugInformationFormat.PortablePdb;
        private bool _optionsInitialized;
        private CSharpParseOptions _parseOptions;
        private CSharpCompilationOptions _compilationOptions;

        public virtual CSharpParseOptions ParseOptions
        {
            get
            {
                EnsureOptions();
                return _parseOptions;
            }
        }

        public virtual CSharpCompilationOptions CSharpCompilationOptions
        {
            get
            {
                EnsureOptions();
                return _compilationOptions;
            }
        }

        public EmitOptions EmitOptions { get; }

        public CSharpCompiler(RazorReferenceManagerImp manager, string appName, bool isDevelopment)
        {
            _referenceManager = manager ?? throw new ArgumentNullException(nameof(manager));
            _appName = appName ?? throw new ArgumentNullException(nameof(appName));
            _isDevelopment = isDevelopment;

            EmitOptions = new EmitOptions(debugInformationFormat: _pdbFormat);
        }

        public CSharpCompilerResult CompileToType(string csharpContent)
        {
            var assemblyName = Path.GetRandomFileName();
            var sourceText = SourceText.From(csharpContent, Encoding.UTF8);
            var syntaxTree = CreateSyntaxTree(sourceText).WithFilePath(assemblyName);
            var compilation = this.CreateCompilation(assemblyName)
                .AddSyntaxTrees(syntaxTree);
            return Emit(compilation);
        }

        private CSharpCompilerResult Emit(Compilation compilation)
        {
            var result = new CSharpCompilerResult();

            var messages = compilation.GetDiagnostics().Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
            if (messages.Any())
            {
                result.ProcessingException = new CSharpCompilerException(messages.Select(f => f.GetMessage()));
            }
            else
            {
                using (var assemblyStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var emitRes = compilation.Emit(
                        assemblyStream,
                        pdbStream,
                        options: EmitOptions
                    );

                    if (!emitRes.Success)
                    {
                        var formatter = new DiagnosticFormatter();
                        var errorMessages = emitRes.Diagnostics
                                                //.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                                                .Select(d => formatter.Format(d));

                        result.ProcessingException = new CSharpCompilerException(errorMessages);
                    }
                    else
                    {
                        assemblyStream.Seek(0, SeekOrigin.Begin);
                        pdbStream.Seek(0, SeekOrigin.Begin);

                        result.Assembly = Assembly.Load(assemblyStream.ToArray(), pdbStream.ToArray());
                        result.CompiledType = result.Assembly.GetExportedTypes().FirstOrDefault();
                    }
                }
            }

            return result;
        }

        private SyntaxTree CreateSyntaxTree(SourceText sourceText)
        {
            return CSharpSyntaxTree.ParseText(
                sourceText,
                options: ParseOptions);
        }

        private Microsoft.CodeAnalysis.CSharp.CSharpCompilation CreateCompilation(string assemblyName)
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
                assemblyName,
                options: CSharpCompilationOptions,
                references: _referenceManager.CompilationReferences);
        }

        // Internal for unit testing.
        protected internal virtual DependencyContextCompilationOptions GetDependencyContextCompilationOptions()
        {
            if (!string.IsNullOrEmpty(_appName))
            {
                var applicationAssembly = Assembly.Load(new AssemblyName(_appName));
                var dependencyContext = DependencyContext.Load(applicationAssembly);
                if (dependencyContext?.CompilationOptions != null)
                {
                    return dependencyContext.CompilationOptions;
                }
            }

            return DependencyContextCompilationOptions.Default;
        }

        private void EnsureOptions()
        {
            if (!_optionsInitialized)
            {
                var dependencyContextOptions = GetDependencyContextCompilationOptions();
                _parseOptions = GetParseOptions(dependencyContextOptions);
                _compilationOptions = GetCompilationOptions(dependencyContextOptions);

                _optionsInitialized = true;
            }
        }

        private CSharpCompilationOptions GetCompilationOptions(DependencyContextCompilationOptions dependencyContextOptions)
        {
            var csharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            // Disable 1702 until roslyn turns this off by default
            csharpCompilationOptions = csharpCompilationOptions.WithSpecificDiagnosticOptions(
                new Dictionary<string, ReportDiagnostic>
                {
                    {"CS1701", ReportDiagnostic.Suppress}, // Binding redirects
                    {"CS1702", ReportDiagnostic.Suppress},
                    {"CS1705", ReportDiagnostic.Suppress}
                });

            if (dependencyContextOptions.AllowUnsafe.HasValue)
            {
                csharpCompilationOptions = csharpCompilationOptions.WithAllowUnsafe(
                    dependencyContextOptions.AllowUnsafe.Value);
            }

            OptimizationLevel optimizationLevel;
            if (dependencyContextOptions.Optimize.HasValue)
            {
                optimizationLevel = dependencyContextOptions.Optimize.Value ?
                    OptimizationLevel.Release :
                    OptimizationLevel.Debug;
            }
            else
            {
                optimizationLevel = _isDevelopment ?
                    OptimizationLevel.Debug :
                    OptimizationLevel.Release;
            }
            csharpCompilationOptions = csharpCompilationOptions.WithOptimizationLevel(optimizationLevel);

            if (dependencyContextOptions.WarningsAsErrors.HasValue)
            {
                var reportDiagnostic = dependencyContextOptions.WarningsAsErrors.Value ?
                    ReportDiagnostic.Error :
                    ReportDiagnostic.Default;
                csharpCompilationOptions = csharpCompilationOptions.WithGeneralDiagnosticOption(reportDiagnostic);
            }

            return csharpCompilationOptions;
        }

        private CSharpParseOptions GetParseOptions(DependencyContextCompilationOptions dependencyContextOptions)
        {
            var configurationSymbol = _isDevelopment ? "DEBUG" : "RELEASE";
            var defines = dependencyContextOptions.Defines.Concat(new[] { configurationSymbol });

            var parseOptions = new CSharpParseOptions(preprocessorSymbols: defines);

            LanguageVersion languageVersion;
            if (!string.IsNullOrEmpty(dependencyContextOptions.LanguageVersion) &&
                Enum.TryParse(dependencyContextOptions.LanguageVersion, ignoreCase: true, result: out languageVersion))
            {
                parseOptions = parseOptions.WithLanguageVersion(languageVersion);
            }

            return parseOptions;
        }
    }
}
