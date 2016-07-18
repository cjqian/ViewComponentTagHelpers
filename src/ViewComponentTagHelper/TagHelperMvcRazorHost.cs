// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentMvcRazorHost : MvcRazorHost
    {
        public ViewComponentMvcRazorHost(
            IChunkTreeCache chunkTreeCache,
            ITagHelperDescriptorResolver tagHelperDescriptorResolver,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
            : base(
                  chunkTreeCache,
                  new ViewComponentTagHelperDescriptorResolver(
                     tagHelperDescriptorResolver,
                     viewComponentDescriptorProvider,
                     compilationService))
        {
        }
    }
}