using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var rootFile = "ViewComponentTagHelpersTemplate.txt";
            _viewComponentTagHelpersGenerator = new ViewComponentTagHelpersGenerator(rootDirectory, rootFile);
        }

            //actually, we want an object with ViewComponentDescriptor and then an actual type
        public IEnumerable<ViewComponentTagHelperDescriptor> GetViewComponentTagHelperDescriptors()
        {
            List<ViewComponentTagHelperDescriptor> viewComponentTagHelperDescriptors = new List<ViewComponentTagHelperDescriptor>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();

            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                var viewComponentName = viewComponentDescriptor.ShortName;
                var tagHelperFile = _viewComponentTagHelpersGenerator.WriteTagHelper(viewComponentDescriptor);

                /* Compile the tagHelperFile in memory and add metadata references to the compilation service. */
                var fileInfo = new DummyFileInfo();
                RelativeFileInfo relativeFileInfo = new RelativeFileInfo(fileInfo,  "./");
                var compilationResult = ((InjectRoslynCompilationService)_compilationService).AppendCompile(relativeFileInfo, tagHelperFile);

                /* Add the type to the viewComponentTagHelperDescriptors */
                Type compiledType = compilationResult.CompiledType;
                var viewComponentTagHelperDescriptor = new ViewComponentTagHelperDescriptor(viewComponentDescriptor, compiledType);
                viewComponentTagHelperDescriptors.Add(viewComponentTagHelperDescriptor);
            }

            //Returns viewComponentDescriptors WITH ViewComponentTagHelperType
            return viewComponentTagHelperDescriptors;
        }

    }
}
