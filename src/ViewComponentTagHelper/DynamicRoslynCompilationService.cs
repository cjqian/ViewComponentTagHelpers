﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{
    public class DynamicRosylnCompilationService : DefaultRoslynCompilationService, ICompilationService
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
        private readonly IOptions<RazorViewEngineOptions> _optionsAccessor;

        public DynamicRosylnCompilationService(
            ApplicationPartManager partManager, 
            IOptions<RazorViewEngineOptions> optionsAccessor, 
            IRazorViewEngineFileProviderAccessor fileProviderAccessor, 
            ILoggerFactory loggerFactory) 
                : base(
                      partManager, 
                      optionsAccessor, 
                      fileProviderAccessor, 
                      loggerFactory
                      )
        {
            _compilationReferences = new List<MetadataReference>();
            _optionsAccessor = optionsAccessor;
            _compilationReferences = base.GetCompilationReferences();
        }

        private void UpdateCompilationReferences()
        {
            _compilationReferences.Concat(base.GetCompilationReferences());
            _compilationReferences = _compilationReferences.Distinct().ToList();
        }

        private void UpdateCompilationReferences(MetadataReference metadataReference)
        {
            _compilationReferences.Add(metadataReference);
            UpdateCompilationReferences();
        }

        public CompilationResult CompileAndAddReference(RelativeFileInfo fileInfo, string compilationContent)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (compilationContent == null)
            {
                throw new ArgumentNullException(nameof(compilationContent));
            }


            // CR: Instead of injecting rosyln compilation service, move this to
            // another class an
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
                references: CompilationReferences);

            compilation = Rewrite(compilation);

            // Add metadata references of the new CSharpCompilation compilation to the references.
            UpdateCompilationReferences(compilation.ToMetadataReference());

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

        protected override IList<MetadataReference> GetCompilationReferences()
        {
            UpdateCompilationReferences();
            return _compilationReferences;
        }

        private IList<MetadataReference> CompilationReferences
        {
            get
            {
                UpdateCompilationReferences();
                return LazyInitializer.EnsureInitialized(
                    ref _compilationReferences,
                    ref _compilationReferencesInitialized,
                    ref _compilationReferencesLock,
                    GetCompilationReferences);
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