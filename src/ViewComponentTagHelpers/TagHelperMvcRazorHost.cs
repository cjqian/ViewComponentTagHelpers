// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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