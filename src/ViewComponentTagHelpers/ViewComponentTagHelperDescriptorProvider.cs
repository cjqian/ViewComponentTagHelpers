using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelperDescriptorProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private ViewComponentTagHelpersGenerator _viewComponentTagHelpersGenerator;
        private ICompilationService _compilationService;
        private Dictionary<string, string> _viewComponentDescriptorFiles;
        public ViewComponentTagHelperDescriptorProvider (IViewComponentDescriptorProvider viewComponentDescriptorProvider, ICompilationService compilationService)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
            _compilationService = compilationService;
            _viewComponentDescriptorFiles = new Dictionary<string, string>();

            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var rootFile = "ViewComponentTagHelpersTemplate.txt";
            _viewComponentTagHelpersGenerator = new ViewComponentTagHelpersGenerator(rootDirectory, rootFile);

            WriteViewComponentTagHelperDescriptorFiles();
        }

        //fills in the _viewComponentDescriptorFiles dictionary
        private void WriteViewComponentTagHelperDescriptorFiles()
        {
            var viewComponentTagHelperDescriptors = _viewComponentDescriptorProvider.GetViewComponents();

            foreach (var viewComponentTagHelperDescriptor in viewComponentTagHelperDescriptors)
            {
                var viewComponentName = viewComponentTagHelperDescriptor.DisplayName; //TODO make sure this checks out
                var tagHelperFile = _viewComponentTagHelpersGenerator.WriteTagHelper(viewComponentName);

                _viewComponentDescriptorFiles.Add(viewComponentName, tagHelperFile);
            }
        }

        //actually, we want an object with ViewComponentDescriptor and then an actual type
        public IEnumerable<ViewComponentTagHelperDescriptor> GetViewComponentTagHelperDescriptors()
        {
            IEnumerable<ViewComponentTagHelperDescriptor> viewComponentTagHelperDescriptors = new List<ViewComponentTagHelperDescriptor>();

            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();

            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                var viewComponentName = viewComponentDescriptor.DisplayName;
                var tagHelperFile = _viewComponentDescriptorFiles[viewComponentName];

                if (tagHelperFile == null)
                {
                    throw new Exception("YO WHAT THIS VIEW COMPONENT DIDN'T EXIST EARLIER");
                }

                var fileInfo = new DummyFileInfo();
                RelativeFileInfo relativeFileInfo = new RelativeFileInfo(fileInfo,  "./");
                var compilationResult = _compilationService.Compile(relativeFileInfo, tagHelperFile);
                Type compiledType = compilationResult.GetType();
                var ViewComponentTagHelperDescriptor = new ViewComponentTagHelperDescriptor(viewComponentDescriptor, compiledType);
                viewComponentTagHelperDescriptors.Append(ViewComponentTagHelperDescriptor);
            }

            //Returns viewComponentDescriptors WITH ViewComponentTagHelperType
            return viewComponentTagHelperDescriptors;
        }

    }
}
