using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{

    public class DynamicRoslynCompilationServiceFactory
    {

        private List<MetadataReference> _referenceCache;
        private IOptions<RazorViewEngineOptions> _optionsAccessor;

        public DynamicRoslynCompilationServiceFactory(IOptions<RazorViewEngineOptions> optionsAccessor)
        {
            _referenceCache = new List<MetadataReference>();
            _optionsAccessor = optionsAccessor;
        }

        public CSharpCompilation CreateCSharpCompilation(
            RelativeFileInfo fileInfo,
            string compilationContent,
            IList<MetadataReference> compilationReferences
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
            var parseOptions = _optionsAccessor.Value.ParseOptions;

            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                path: assemblyName,
                options: parseOptions);


            var compilationOptions = _optionsAccessor.Value.CompilationOptions;
            var compilation = CSharpCompilation.Create(
                assemblyName,
                options: compilationOptions,
                syntaxTrees: new[] { syntaxTree },
                references: compilationReferences);

            compilation = Rewrite(compilation);
            return compilation;
        }

        // If have not been added, add to compilation cache. 
        public void AddToReferenceCache(MetadataReference metadataReference){
            _referenceCache.Add(metadataReference);
        }

        public IList<MetadataReference> GetReferenceCache()
        {
            return _referenceCache;
        }

        private CSharpCompilation Rewrite(CSharpCompilation compilation)
        {
            var rewrittenTrees = new List<SyntaxTree>();
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree, ignoreAccessibility: true);
                var rewriter = new ExpressionRewriter(semanticModel);

                var rewrittenTree = tree.WithRootAndOptions(rewriter.Visit(tree.GetRoot()), tree.Options);
                rewrittenTrees.Add(rewrittenTree);
            }

            return compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(rewrittenTrees);
        }
    }
}
