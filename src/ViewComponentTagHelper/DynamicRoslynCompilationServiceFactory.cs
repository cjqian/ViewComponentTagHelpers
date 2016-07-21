/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{

    // This factory creates compilations from files.
    // Also, it keeps a cache of all compilation references.
    public class DynamicRoslynCompilationServiceFactory
    {

        private readonly DebugInformationFormat _pdbFormat =
#if NET451
                SymbolsUtility.SupportsFullPdbGeneration() ?
                    DebugInformationFormat.Pdb :
                    DebugInformationFormat.PortablePdb;
#else
                DebugInformationFormat.PortablePdb;
#endif
        private List<MetadataReference> _referenceCache;
        private IOptions<RazorViewEngineOptions> _optionsAccessor;

        public DynamicRoslynCompilationServiceFactory(IOptions<RazorViewEngineOptions> optionsAccessor)
        {
            _referenceCache = new List<MetadataReference>();
            _optionsAccessor = optionsAccessor;
        }
        public CompilationResult AddReferenceAndCompile(RelativeFileInfo fileInfo, string compilationContent)
        {
            // Create a compilation. 
            var compilation = CreateCSharpCompilation(fileInfo,
                compilationContent,
                GetReferenceCache()
                );

            // Add metadata references of the new CSharpCompilation compilation to the references.
            AddToReferenceCache(compilation.ToMetadataReference());

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
                    }

                    assemblyStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);

                    var assembly = LoadStream(assemblyStream, pdbStream);
                    var type = assembly.GetExportedTypes().FirstOrDefault(a => !a.IsNested);

                    return new CompilationResult(type);
                }
            }
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
    }
}
*/