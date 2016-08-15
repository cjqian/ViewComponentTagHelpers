/*!
        [Microsoft.AspNetCore.Razor.TagHelpers.HtmlTargetElement("vc:dan-roth", TagStructure = Microsoft.AspNetCore.Razor.TagHelpers.TagStructure.NormalOrSelfClosing)]
        private class DanRothViewComponentTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
        {
            private readonly IViewComponentHelper _viewComponentHelper;
            public DanRothViewComponentTagHelper(IViewComponentHelper viewComponentHelper)
            {
                _viewComponentHelper = viewComponentHelper;
            }

            [ViewContext]
            public ViewContext ViewContext { get; set; }
            public System.String jacketColor { get; set; }

            public override async Task ProcessAsync(Microsoft.AspNetCore.Razor.TagHelpers.TagHelperContext context, Microsoft.AspNetCore.Razor.TagHelpers.TagHelperOutput output)
            {
                ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
                var viewContent = await _viewComponentHelper.InvokeAsync("DanRoth", new { jacketColor });
                output.TagName = null;
                output.Content.SetHtmlContent(viewContent);
            }
        }
    }
}*/
