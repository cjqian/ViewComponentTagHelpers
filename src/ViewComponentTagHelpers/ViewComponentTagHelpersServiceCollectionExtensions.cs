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
