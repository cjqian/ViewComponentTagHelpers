﻿/*
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelper
{
    // HtmlTargetElement = 0
    [HtmlTargetElement("vc:[[0]]", TagStructure = TagStructure.WithoutEndTag)]
    // ViewComponentName = 1 
    public class [[1]]ViewComponentTagHelper: TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;

    public [[1]]ViewComponentTagHelper(IViewComponentHelper viewComponentHelper)
    {
        _viewComponentHelper = viewComponentHelper;
    }

    [ViewContext]
    public ViewContext ViewContext { get; set; }

    // CustomParameters = 2
    [[2]]
		
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
        // ParametersObject = 3
        var viewContent = await _viewComponentHelper.InvokeAsync("[[1]]", new {[[3]]});	
			output.Content.SetHtmlContent(viewContent);
        }
    }
}*/