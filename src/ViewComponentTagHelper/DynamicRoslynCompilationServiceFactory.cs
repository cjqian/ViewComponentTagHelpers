using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{

    public class DynamicRoslynCompilationServiceFactory
    {
        public DynamicRoslynCompilationServiceFactory()
        {
        }

        public CSharpCompilation CreateCSharpCompilation(
            RelativeFileInfo fileInfo, 
            string compilationContent, 
            IList<MetadataReference> compilationReferences,
            IOptions<RazorViewEngineOptions> optionsAccessor
            )
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (compilationContent == null)
            {
                throw new ArgumentNullException(nameof(compilationContent));
            }

            // Creates a CSharpCompilation using arguments in preparation of compilation. 
            var assemblyName = Path.GetRandomFileName();
            var parseOptions = optionsAccessor.Value.ParseOptions;

            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                path: assemblyName,
                options: parseOptions);


            var compilationOptions = optionsAccessor.Value.CompilationOptions;
            var compilation = CSharpCompilation.Create(
                assemblyName,
                options: compilationOptions,
                syntaxTrees: new[] { syntaxTree },
                references: compilationReferences);

            return compilation;
        } 
    }
}
