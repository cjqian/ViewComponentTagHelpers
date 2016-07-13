// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelperDescriptorProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private ViewComponentTagHelpersGenerator _viewComponentTagHelpersGenerator;
        private ICompilationService _compilationService;

        public ViewComponentTagHelperDescriptorProvider (IViewComponentDescriptorProvider viewComponentDescriptorProvider, ICompilationService compilationService)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
            _compilationService = compilationService;

            //Location of the ViewCOmponentTagHelpersTemplate; will eventually generate without template.
            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var rootFile = "ViewComponentTagHelpersTemplate.txt";

            _viewComponentTagHelpersGenerator = new ViewComponentTagHelpersGenerator(rootDirectory, rootFile);
        }

        /// <summary>
        /// Returns a list of ViewCOmponentTagHelperDescriptors, which include the type of the compilation and the view component descriptor. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ViewComponentTagHelperDescriptor> GetViewComponentTagHelperDescriptors()
        {
            List<ViewComponentTagHelperDescriptor> viewComponentTagHelperDescriptors = new List<ViewComponentTagHelperDescriptor>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                //Generates a tagHelperFile (string .cs tag helper equivalent of the tag helper.)
                var tagHelperFile = _viewComponentTagHelpersGenerator.WriteTagHelper(viewComponentDescriptor);

                //Compile the tagHelperFile in memory and add metadata references to the compilation service.
                var fileInfo = new DummyFileInfo();
                RelativeFileInfo relativeFileInfo = new RelativeFileInfo(fileInfo,  "./");
                var compilationResult = ((InjectRoslynCompilationService)_compilationService).AppendCompile(relativeFileInfo, tagHelperFile);

                //Add a viewcomponenttaghelperdescriptor to our list.
                var viewComponentTagHelperDescriptor = new ViewComponentTagHelperDescriptor(viewComponentDescriptor, compilationResult.compiledType);
                viewComponentTagHelperDescriptors.Add(viewComponentTagHelperDescriptor);
            }

            return viewComponentTagHelperDescriptors;
        }
    }
}
