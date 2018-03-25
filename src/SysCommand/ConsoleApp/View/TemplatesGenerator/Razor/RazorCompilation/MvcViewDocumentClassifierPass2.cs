using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class MvcViewDocumentClassifierPass2 : DocumentClassifierPassBase
    {
        public static readonly string MvcViewDocumentKind = "mvc.1.0.view";
        private readonly string runtimeTemplatesNamespace;
        private readonly string baseTypeName;

        protected override string DocumentKind => MvcViewDocumentKind;

        protected override bool IsMatch(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode) => true;

        public MvcViewDocumentClassifierPass2(string runtimeTemplatesNamespace, string baseTypeName)
        {
            this.runtimeTemplatesNamespace = runtimeTemplatesNamespace;
            this.baseTypeName = baseTypeName;
        }

        protected override void OnDocumentStructureCreated(
            RazorCodeDocument codeDocument,
            NamespaceDeclarationIntermediateNode @namespace,
            ClassDeclarationIntermediateNode @class,
            MethodDeclarationIntermediateNode method)
        {
            var relative = codeDocument.Items["relative-path"] as string;
            var filePath = relative ?? codeDocument.Source.FilePath;
            base.OnDocumentStructureCreated(codeDocument, @namespace, @class, method);
            @namespace.Content = runtimeTemplatesNamespace;
            @class.ClassName = CSharpIdentifier.GetClassNameFromPath(filePath);
            @class.BaseType = baseTypeName;
            @class.Modifiers.Clear();
            @class.Modifiers.Add("public");

            method.MethodName = "ExecuteAsync";
            method.Modifiers.Clear();
            method.Modifiers.Add("public");
            method.Modifiers.Add("async");
            method.Modifiers.Add("override");
            method.ReturnType = $"global::{typeof(System.Threading.Tasks.Task).FullName}";
        }
    }
}
