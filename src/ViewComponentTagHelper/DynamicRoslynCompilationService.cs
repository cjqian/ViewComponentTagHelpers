// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{
    public class DynamicRosylnCompilationService : DefaultRoslynCompilationService, ICompilationService
    {
        private readonly DebugInformationFormat _pdbFormat =
#if NET451
                SymbolsUtility.SupportsFullPdbGeneration() ?
                    DebugInformationFormat.Pdb :
                    DebugInformationFormat.PortablePdb;
#else
                DebugInformationFormat.PortablePdb;
#endif
        private readonly IOptions<RazorViewEngineOptions> _optionsAccessor;
        private DynamicRoslynCompilationServiceFactory _dynamicRoslynCompilationServiceFactory;

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
            _optionsAccessor = optionsAccessor;
            _dynamicRoslynCompilationServiceFactory = new DynamicRoslynCompilationServiceFactory(optionsAccessor);
        }

        public CompilationResult AddReferenceAndCompile(RelativeFileInfo fileInfo, string compilationContent)
        {
            // Create a compilation. 
            var compilation = _dynamicRoslynCompilationServiceFactory.CreateCSharpCompilation(fileInfo,
                compilationContent,
                GetCompilationReferences()
                );

            // Add metadata references of the new CSharpCompilation compilation to the references.
            _dynamicRoslynCompilationServiceFactory.AddToReferenceCache(compilation.ToMetadataReference());

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

        protected override IList<MetadataReference> GetCompilationReferences()
        {
            var references = _dynamicRoslynCompilationServiceFactory.GetReferenceCache();
            references = references.Concat(base.GetCompilationReferences()).Distinct().ToList();

            return references;
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
