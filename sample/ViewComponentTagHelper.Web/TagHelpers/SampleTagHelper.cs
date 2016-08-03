using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelper.Web
{
    [HtmlTargetElement("sample", TagStructure=TagStructure.NormalOrSelfClosing)]
    public class SampleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            output.Content.SetHtmlContent("I WORK");
        }
    }
}
