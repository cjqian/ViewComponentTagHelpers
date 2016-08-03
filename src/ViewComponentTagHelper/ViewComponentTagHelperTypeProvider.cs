// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private readonly ViewComponentTagHelperGenerator _viewComponentTagHelperGenerator;

        private readonly ReferenceManager _referenceManager;
        private readonly ViewComponentCompilationService _viewComponentCompilationService;

        private readonly Dictionary<string, TypeInfo> _compiledTypes;

        public ViewComponentTagHelperTypeProvider(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ViewComponentCompilationService viewComponentCompilationService,
            ReferenceManager referenceManager)
        {
            _referenceManager = referenceManager;

            _viewComponentCompilationService = viewComponentCompilationService;
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;

            _compiledTypes = new Dictionary<string, TypeInfo>();

            // TODO: put all classes together so compile/make references once?
            // TODO: embed or write out individual template

            _viewComponentTagHelperGenerator = new ViewComponentTagHelperGenerator();
        }

        // Creates a tag helper for each view component. 
        public IEnumerable<TypeInfo> GetTagHelperTypes()
        {
            var tagHelperTypes = new List<TypeInfo>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                if (!_compiledTypes.ContainsKey(viewComponentDescriptor.FullName))
                {
                    // Generates a "fake" tag helper class from the view component and compile.
                    var tagHelperFile = _viewComponentTagHelperGenerator.WriteTagHelper(viewComponentDescriptor);
                    var compilation = _viewComponentCompilationService.CreateCSharpCompilation(
                        new RelativeFileInfo(new DummyFileInfo(), "./"),
                        tagHelperFile
                        );
                    var compilationResult = _viewComponentCompilationService.Compile(compilation);

                    // Add reference to compilation to global list, and store so we no longer need to compile this time.
                    _referenceManager.AddReference(compilation.ToMetadataReference());
                    _compiledTypes[viewComponentDescriptor.FullName] = compilationResult.CompiledType.GetTypeInfo();
                }

                tagHelperTypes.Add(_compiledTypes[viewComponentDescriptor.FullName]);
            }

            return tagHelperTypes;
        }
    }
}