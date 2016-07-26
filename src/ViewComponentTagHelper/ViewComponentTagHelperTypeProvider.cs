// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;

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

        // Creates a tag helper for each view component and caches. 
        public IEnumerable<TypeInfo> GetTagHelperTypes()
        {
            var tagHelperTypes = new List<TypeInfo>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                if (!_compiledTypes.ContainsKey(viewComponentDescriptor.FullName))
                {
                    var fileInfo = new DummyFileInfo();
                    var relativeFileInfo = new RelativeFileInfo(fileInfo, "./");

                    // Generates a tagHelperFile (string .cs tag helper equivalent of the tag helper.)
                    var tagHelperFile = _viewComponentTagHelperGenerator.WriteTagHelper(viewComponentDescriptor);
                    var compilation = _viewComponentCompilationService.CreateCSharpCompilation(relativeFileInfo, tagHelperFile);
                    var compilationResult = _viewComponentCompilationService.Compile(compilation);

                    _referenceManager.AddReference(compilation.ToMetadataReference());

                    _compiledTypes[viewComponentDescriptor.FullName] = compilationResult.CompiledType.GetTypeInfo();
                    var curType = compilationResult.CompiledType;
                    var curTypeInfo = curType.GetTypeInfo();
                }
        
                tagHelperTypes.Add(_compiledTypes[viewComponentDescriptor.FullName]);
            }

            if (tagHelperTypes.Count == 0)
            {
                return Enumerable.Empty<TypeInfo>();
            }

            return tagHelperTypes;
        }
    }
}