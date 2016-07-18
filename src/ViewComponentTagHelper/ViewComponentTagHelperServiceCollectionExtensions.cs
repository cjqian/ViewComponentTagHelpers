// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace ViewComponentTagHelper
{
    public static class ViewComponentTagHelpererviceCollectionExtensions
    {
        public static IServiceCollection AddViewComponentTagHelper(this IServiceCollection services)
        {
            services.AddTransient<IMvcRazorHost, ViewComponentMvcRazorHost>();
            services.AddSingleton<ICompilationService, InjectRoslynCompilationService>();
            //services.AddSingleton<ITagHelperTypeResolver, ViewComponentTagHelperTypeResolver>();
            return services;
        }
    }
}
