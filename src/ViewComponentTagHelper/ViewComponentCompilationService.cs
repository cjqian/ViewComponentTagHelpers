using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{
    public class ViewComponentCompilationService
    {
        private IOptions<RazorViewEngineOptions> _optionsAccessor;
        private readonly DebugInformationFormat _pdbFormat =
#if NET451
                SymbolsUtility.SupportsFullPdbGeneration() ?
                    DebugInformationFormat.Pdb :
                    DebugInformationFormat.PortablePdb;
#else
                DebugInformationFormat.PortablePdb;
#endif
        private ReferenceManager _referenceManager;
        private IViewComponentDescriptorProvider _viewComponentDescriptorProvider;

        public ViewComponentCompilationService(
            IOptions<RazorViewEngineOptions> optionsAccessor,
            ReferenceManager referenceManager,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider
            )
        {
            _optionsAccessor = optionsAccessor;
            _referenceManager = referenceManager;
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
        }
        
        public CSharpCompilation CreateCSharpCompilation(RelativeFileInfo fileInfo, string compilationContent)
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
                references: _referenceManager.GetReferences());

            compilation = Rewrite(compilation);
            return compilation;
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

        private Assembly LoadStream(MemoryStream assemblyStream, MemoryStream pdbStream)
        {
#if NET451
            return Assembly.Load(assemblyStream.ToArray(), pdbStream.ToArray());
#else
            return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(assemblyStream, pdbStream);
#endif
        }

        public CompilationResult Compile(CSharpCompilation compilation)
        {
            // Create the compilation.
            var compilationContext = new RoslynCompilationContext(compilation);
            var compilationCallback = _optionsAccessor.Value.CompilationCallback;
            compilationCallback(compilationContext);
            compilation = compilationContext.Compilation;

            // Compiles roughly in agreeance to DefaultRoslynCompilationService's Compile() //
            using (var assemblyStream = new MemoryStream())
            {
                using (var pdbStream = new MemoryStream())
                {
                    var result = compilation.Emit(
                        assemblyStream,
                        pdbStream,
                        options: new EmitOptions(debugInformationFormat: _pdbFormat)
                       );

                    if (!result.Success)
                    {
                        throw new Exception("Compilation failed. Diagnostics available.");
                        /* TODO: Write unit testing path for compilation failed result. */
                        /* fileInfo.RelativePath. */
                        /* compilationContent */
                        /* assemblyName */
                        /* result.Diagnostics */
                    }

                    assemblyStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);

                    var assembly = LoadStream(assemblyStream, pdbStream);
                    var type = assembly.GetExportedTypes().FirstOrDefault(a => !a.IsNested);

                    return new CompilationResult(type);
                }
            }
        } 
    }
}
