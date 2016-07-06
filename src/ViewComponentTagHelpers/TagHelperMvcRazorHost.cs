using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelpers
{
    public class TagHelperMvcRazorHost : MvcRazorHost
    {
        public TagHelperMvcRazorHost(
            IChunkTreeCache chunkTreeCache,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider)
            : base(chunkTreeCache, new ViewComponentTagHelpersDescriptorResolver(new TagHelperTypeResolver(), viewComponentDescriptorProvider))
        {
            TagHelperDescriptorResolver = new ViewComponentTagHelpersDescriptorResolver(
                new TagHelperTypeResolver(),
                viewComponentDescriptorProvider);
        }
    }
}