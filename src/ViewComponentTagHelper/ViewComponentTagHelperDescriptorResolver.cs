using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    // TODO: Yoooo, we have to overwrite the existing TagHelperDescriptorResolver. 
    public class ViewComponentTagHelperDescriptorResolver : TagHelperDescriptorResolver
    {
        private ViewComponentTagHelperDescriptorFactory _descriptorFactory;
        public ViewComponentTagHelperDescriptorResolver(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider)
            : base ( designTime: false ) 
        {
            _descriptorFactory = new ViewComponentTagHelperDescriptorFactory(viewComponentDescriptorProvider);
        }

        protected override IEnumerable<TagHelperDescriptor> ResolveDescriptorsInAssembly(
            string assemblyName, 
            SourceLocation documentLocation, 
            ErrorSink errorSink)
        {
            var tagHelperDescriptors = base.ResolveDescriptorsInAssembly(
                assemblyName, 
                documentLocation, 
                errorSink);

            var viewComponentTagHelperDescriptors = _descriptorFactory.CreateDescriptors(assemblyName, null, errorSink);
            var descriptors = tagHelperDescriptors.Concat(viewComponentTagHelperDescriptors);
            return descriptors;
        }
    }
}
