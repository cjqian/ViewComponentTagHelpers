using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelper
{
    // HtmlTargetElement = 0
    [HtmlTargetElement("vc:dan-roth")]
    // ViewComponentName = 1 
    public class DanRothViewComponentTagHelper : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;

        public DanRothViewComponentTagHelper(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        // CustomParameters = 2
        public String jacketColor { get; set; }
        public Int32 age { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            // ParametersObject = 3
            var viewContent = await _viewComponentHelper.InvokeAsync("DanRoth", new { jacketColor, age });
            output.Content.SetHtmlContent(viewContent);
        }
    }
}
