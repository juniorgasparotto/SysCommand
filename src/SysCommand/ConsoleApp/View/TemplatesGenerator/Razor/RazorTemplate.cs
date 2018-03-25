using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using SysCommand.ConsoleApp.Helpers;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    public class RazorTemplate
    {
        private readonly CSharpCompiler compiler;
        private readonly RazorViewExecutor razorViewExecutor;
        private readonly RazorReferenceManagerImp referenceManager;

        private string BasePath => Directory.GetCurrentDirectory();
        private string RuntimeTemplatesNamespace => $"{nameof(Razor)}.RuntimeTemplates";
        private string BaseViewName => $"global::{CSharpIdentifier.GetFullNameWithoutGenerics<RazorView<object>>()}<TModel>";
        
        public RazorTemplate()
        {
            var curAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            this.referenceManager = new RazorReferenceManagerImp(AssemblyDiscovery.GetAllLoaded());
            this.compiler = new CSharpCompiler(this.referenceManager, curAssemblyName, Development.IsAttached);
            this.razorViewExecutor = new RazorViewExecutor();
        }

        public string Parse<T>(string viewPath, T model)
        {
            return ParseAsync<T>(viewPath, model).Result;
        }

        public string Parse(string viewPath)
        {
            return ParseAsync(viewPath).Result;
        }

        public string ParseContent<T>(string content, T model)
        {
            return ParseContentAsync<T>(content, model).Result;
        }

        public string ParseContent(string viewPath)
        {
            return ParseContentAsync(viewPath).Result;
        }

        public async Task<string> ParseAsync(string viewPath)
        {
            return await ParseAsync(viewPath, new { });
        }

        public async Task<string> ParseAsync<T>(string viewPath, T model)
        {
            var razorCompilerResult = ToCSharpByPath(viewPath);
            return await ParseResultAsync(model, razorCompilerResult);
        }

        public async Task<string> ParseContentAsync(string content)
        {
            return await ParseContentAsync(content, new { });
        }

        public async Task<string> ParseContentAsync<T>(string content, T model)
        {
            var razorCompilerResult = ToCSharpByContent(content);
            return await ParseResultAsync(model, razorCompilerResult);
        }

        private async Task<string> ParseResultAsync<T>(T model, RazorCompilerResult razorCompilerResult)
        {
            if (razorCompilerResult.ProcessingException != null)
                throw razorCompilerResult.ProcessingException;

            var csharpCompilerResult = compiler.CompileToType(razorCompilerResult.GeneratedText);

            if (csharpCompilerResult.ProcessingException != null)
                throw csharpCompilerResult.ProcessingException;

            return await razorViewExecutor.ExecuteAsync<T>(csharpCompilerResult.CompiledType, model);
        }

        internal RazorCompilerResult ToCSharpByPath(string viewPath)
        {
            if (!File.Exists(viewPath))
                throw new FileNotFoundException(viewPath);

            var razorEngine = GetRazorEngine();
            var razorProject = RazorProject.Create(BasePath);
            var cshtmlFile = razorProject.GetItem(viewPath);
            var projectItem = new FileSystemRazorProjectItem(cshtmlFile);

            var templateEngine = GetTemplateEngine(razorEngine, razorProject);

            return GetCompilerResult(templateEngine, projectItem);
        }

        internal RazorCompilerResult ToCSharpByContent(string content)
        {
            var razorEngine = GetRazorEngine();
            var razorProject = RazorProject.Create(BasePath);
            var projectItem = new ContentRazorProjectItem(content);
            var templateEngine = GetTemplateEngine(razorEngine, razorProject);
            return GetCompilerResult(templateEngine, projectItem);
        }

        private RazorEngine GetRazorEngine()
        {
            var razorEngine = RazorEngine.Create(builder =>
            {
                InjectDirective.Register(builder);
                ModelDirective.Register(builder);
                NamespaceDirective.Register(builder);
                PageDirective.Register(builder);
                FunctionsDirective.Register(builder);
                InheritsDirective.Register(builder);
                SectionDirective.Register(builder);

                ////builder.AddTargetExtension(new TemplateTargetExtension()
                ////{
                ////    TemplateTypeName = "global::Microsoft.AspNetCore.Mvc.Razor.HelperResult",
                ////});

                ////builder.Features.Add(new SuppressChecksumOptionsFeature());
                builder.Features.Add(new ModelExpressionPass());
                builder.Features.Add(new PagesPropertyInjectionPass());
                builder.Features.Add(new ViewComponentTagHelperPass());
                builder.Features.Add(new RazorPageDocumentClassifierPass());
                builder.Features.Add(new MvcViewDocumentClassifierPass2(RuntimeTemplatesNamespace, BaseViewName));
                builder.Features.Add(new AssemblyAttributeInjectionPass2());

                if (!builder.DesignTime)
                {
                    //builder.Features.Add(new InstrumentationPass());
                }
            });

            return razorEngine;
        }

        private RazorTemplateEngine GetTemplateEngine(RazorEngine razorEngine, RazorProject razorProject)
        {
            var templateEngine = new RazorTemplateEngine(razorEngine, razorProject);
            templateEngine.Options.DefaultImports = RazorSourceDocument.Create(@"
        @using System
        @using System.Threading.Tasks
        ", fileName: null);
            return templateEngine;
        }

        private RazorCompilerResult GetCompilerResult(
            RazorTemplateEngine templateEngine,
            RazorProjectItem projectItem)
        {
            var cSharpDocument = templateEngine.GenerateCode(projectItem);

            var result = new RazorCompilerResult();
            if (cSharpDocument.Diagnostics.Any())
            {
                var messages = cSharpDocument.Diagnostics.Select(d => d.GetMessage());
                result.ProcessingException = new RazorCompilerException(messages, cSharpDocument.GeneratedCode);
            }
            else
            {
                result.GeneratedText = cSharpDocument.GeneratedCode;
            }
            return result;
        }
    }
}
