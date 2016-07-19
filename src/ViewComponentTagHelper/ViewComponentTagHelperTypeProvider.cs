// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private readonly ViewComponentTagHelperGenerator _ViewComponentTagHelperGenerator;
        private readonly InjectRoslynCompilationService _compilationService;
        private readonly Dictionary<string, Type> _compiledTagHelperCache;

        public ViewComponentTagHelperTypeProvider(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
            _compilationService = (InjectRoslynCompilationService)compilationService;
            _compiledTagHelperCache = new Dictionary<string, Type>();
            // TODO: put all classes together so compile/make references once?
            // TODO: embed or write out individual template

            _ViewComponentTagHelperGenerator = new ViewComponentTagHelperGenerator();
        }

        public ViewComponentTagHelperTypeWrapper GetTagHelperTypeWrappers()
        {
            UpdateTagHelperTypes();

            var namespaceList = new List<String>();

            foreach (var type in _compiledTagHelperCache.Values)
            {
                if (!namespaceList.Contains(type.Namespace))
                {
                    namespaceList.Add(type.Namespace);
                }
            }

            var viewComponentTagHelperTypeWrapper = new ViewComponentTagHelperTypeWrapper(_compiledTagHelperCache.Values, namespaceList);
            return viewComponentTagHelperTypeWrapper;
        }
        
        private void UpdateTagHelperTypes()
        {
            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                if (!_compiledTagHelperCache.ContainsKey(viewComponentDescriptor.ShortName))
                {
                // Compile the tagHelperFile in memory and add metadata references to the compilation service.
                var fileInfo = new DummyFileInfo();
                var relativeFileInfo = new RelativeFileInfo(fileInfo, "./");

                // Generates a tagHelperFile (string .cs tag helper equivalent of the tag helper.)
                var tagHelperFile = _ViewComponentTagHelperGenerator.WriteTagHelper(viewComponentDescriptor);

                var compilationResult = _compilationService.CompileAndAddReference(relativeFileInfo, tagHelperFile);
                    _compiledTagHelperCache[viewComponentDescriptor.ShortName] = compilationResult.CompiledType;
                }
            }
        }
    }
}
