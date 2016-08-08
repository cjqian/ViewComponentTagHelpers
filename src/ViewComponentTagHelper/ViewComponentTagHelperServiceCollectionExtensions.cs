// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace ViewComponentTagHelper
{
    public static class ViewComponentTagHelpererviceCollectionExtensions
    {
        public static IServiceCollection AddViewComponentTagHelper(this IServiceCollection services)
        {
            services.AddSingleton<ICompilationService, DynamicRosylnCompilationService>();
            services.AddSingleton<ITagHelperDescriptorResolver, ViewComponentTagHelperDescriptorResolver>();
            return services;
        }
    }
}
