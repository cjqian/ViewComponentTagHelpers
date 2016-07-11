using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelpers
{
    public class TagHelperMvcRazorHost : MvcRazorHost
    {
        public TagHelperMvcRazorHost(
            IChunkTreeCache chunkTreeCache,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
            : base(chunkTreeCache, new ViewComponentTagHelpersDescriptorResolver(new TagHelperTypeResolver(), viewComponentDescriptorProvider, compilationService))
        {
            TagHelperDescriptorResolver = new ViewComponentTagHelpersDescriptorResolver(
                new TagHelperTypeResolver(),
                viewComponentDescriptorProvider,
                compilationService);
        }
    }
}