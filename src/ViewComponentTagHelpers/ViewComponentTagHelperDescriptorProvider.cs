// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelperDescriptorProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private readonly ViewComponentTagHelpersGenerator _viewComponentTagHelpersGenerator;
        private readonly InjectRoslynCompilationService _compilationService;

        public ViewComponentTagHelperDescriptorProvider (IViewComponentDescriptorProvider viewComponentDescriptorProvider, ICompilationService compilationService)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
            _compilationService = (InjectRoslynCompilationService)compilationService;

            // TODO: put all classes together so compile/make references once
            // TODO: embed or write out individual template

            _viewComponentTagHelpersGenerator = new ViewComponentTagHelpersGenerator();
        }

        public IEnumerable<Type> GetTagHelperTypes()
        {
            var tagHelperTypes = new List<Type>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                //Generates a tagHelperFile (string .cs tag helper equivalent of the tag helper.)
                var tagHelperFile = _viewComponentTagHelpersGenerator.WriteTagHelper(viewComponentDescriptor);

                //Compile the tagHelperFile in memory and add metadata references to the compilation service.
                var fileInfo = new DummyFileInfo();
                RelativeFileInfo relativeFileInfo = new RelativeFileInfo(fileInfo,  "./");
                var compilationResult = _compilationService.CompileAndAddReference(relativeFileInfo, tagHelperFile);

                tagHelperTypes.Add(compilationResult.CompiledType);
            }

            return tagHelperTypes;
        }
    }
}
