﻿using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelpers
{
    [HtmlTargetElement(Attributes = VisiblePropertyName)]
    public class VisibleAttributeTagHelper : TagHelper
    {
        private const string VisiblePropertyName = "asp-visible";

        [HtmlAttributeName(VisiblePropertyName)]
        public bool Visible { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Visible)
            {
                output.SuppressOutput();
            }
        }
    }
}