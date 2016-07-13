// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace ViewComponentTagHelpers
{
    public static class ViewComponentTagHelpersServiceCollectionExtensions
    {
        public static IServiceCollection AddViewComponentTagHelpers(this IServiceCollection services)
        {
            services.AddTransient<IMvcRazorHost, TagHelperMvcRazorHost>();

            return services;
        }
    }
}
