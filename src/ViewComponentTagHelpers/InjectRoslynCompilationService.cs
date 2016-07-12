using Microsoft.AspNetCore.Mvc.Razor.Internal;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.Emit;

namespace ViewComponentTagHelpers
{
    public class InjectRoslynCompilationService : DefaultRoslynCompilationService, ICompilationService 
    {
        private IList<MetadataReference> _compilationReferences;
        private bool _compilationReferencesInitialized;
        private object _compilationReferencesLock = new object();
        private readonly DebugInformationFormat _pdbFormat =
#if NET451
            SymbolsUtility.SupportsFullPdbGeneration() ?
                DebugInformationFormat.Pdb :
                DebugInformationFormat.PortablePdb;
#else
            DebugInformationFormat.PortablePdb;
#endif
        private IOptions<RazorViewEngineOptions> _optionsAccessor;

        public InjectRoslynCompilationService(ApplicationPartManager partManager, IOptions<RazorViewEngineOptions> optionsAccessor, IRazorViewEngineFileProviderAccessor fileProviderAccessor, ILoggerFactory loggerFactory) : base(partManager, optionsAccessor, fileProviderAccessor, loggerFactory)
        {
            _compilationReferences = new List<MetadataReference>();
            _optionsAccessor = optionsAccessor;
            _compilationReferences = base.GetCompilationReferences();
        }

         CompilationResult ICompilationService.Compile(RelativeFileInfo fileInfo, string compilationContent)
        {

            return base.Compile(fileInfo, compilationContent);
        }

        public void UpdateCompilationReferences()
        {
            _compilationReferences.Concat(base.GetCompilationReferences());
            _compilationReferences = _compilationReferences.Distinct().ToList();
        }

        public void UpdateCompilationReferences(MetadataReference metadataReference)
        {
            _compilationReferences.Add(metadataReference);
            UpdateCompilationReferences();
        }

         
        //Compiles and appends metadatareferences to the existing list
        public CompilationResult AppendCompile(RelativeFileInfo fileInfo, string compilationContent)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (compilationContent == null)
            {
                throw new ArgumentNullException(nameof(compilationContent));
            }

            var assemblyName = Path.GetRandomFileName();

            var compilationCallback = _optionsAccessor.Value.CompilationCallback;
            var compilationOptions = _optionsAccessor.Value.CompilationOptions;
            var parseOptions = _optionsAccessor.Value.ParseOptions;

            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                path: assemblyName,
                options: parseOptions);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                options: compilationOptions,
                syntaxTrees: new[] { syntaxTree },
                references: CompilationReferences);

            compilation = Rewrite(compilation);

            //AT THIS POINT, ADD THE METADATA REFERENCES TO THE LIST
            UpdateCompilationReferences(compilation.ToMetadataReference());

            var compilationContext = new RoslynCompilationContext(compilation);
            compilationCallback(compilationContext);
            compilation = compilationContext.Compilation;

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

        protected override IList<MetadataReference> GetCompilationReferences()
        {
            UpdateCompilationReferences();
            return _compilationReferences;
        }

         IList<MetadataReference> CompilationReferences
        {
            get
            {
                UpdateCompilationReferences();
                return LazyInitializer.EnsureInitialized(
                    ref _compilationReferences,
                    ref _compilationReferencesInitialized,
                    ref _compilationReferencesLock,
                    GetCompilationReferences
                    );
            }
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

    }
}
