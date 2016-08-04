namespace ViewComponentTagHelper.Web
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    // HtmlTargetElement = 0
    [HtmlTargetElement("vc:about", TagStructure = TagStructure.NormalOrSelfClosing)]
    // ViewComponentName = 1 
    public class AboutViewComponentTagHelper : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;

        public AboutViewComponentTagHelper(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        // CustomParameters = 2
        public String email { get; set; }
        public String phoneNumber { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            // ParametersObject = 3
            var viewContent = await _viewComponentHelper.InvokeAsync("About", new { email, phoneNumber });

            output.TagName = "";
            output.Content.SetHtmlContent(viewContent);
        }
    }
}