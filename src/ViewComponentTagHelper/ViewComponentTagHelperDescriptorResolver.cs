using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    // CR: Wrapper instead of overwriting existing TagHelperDescriptorResolver.
    // CR: ITagHelperDescriptorResolver type now.
    public class ViewComponentTagHelperDescriptorResolver : TagHelperDescriptorResolver
    {
        private ViewComponentTagHelperDescriptorFactory _descriptorFactory;
        
        public ViewComponentTagHelperDescriptorResolver(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider)
            : base ( designTime: false ) // Note: not entirely sure about this base thing. CR:
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

            var viewComponentTagHelperDescriptors = _descriptorFactory.ResolveDescriptorsInAssembly(assemblyName);

            var descriptors = tagHelperDescriptors.Concat(viewComponentTagHelperDescriptors);
            return descriptors;
        }
    }
}
