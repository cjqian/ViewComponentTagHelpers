// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ViewComponentTagHelpers
{
    /// <summary>
    /// Resolves ViewComponentTagHelperDescriptors.
    /// </summary>
    public class ViewComponentTagHelpersDescriptorResolver : TagHelperDescriptorResolver, ITagHelperDescriptorResolver
    {
        private readonly ViewComponentTagHelperDescriptorProvider _viewComponentTagHelperDescriptorProvider;
        private IEnumerable<TagHelperDescriptor> _viewComponentTagHelpersDescriptors;
        private ICompilationService _compilationService;

        /// <summary>
        /// Creates an instance of the ViewComponentTagHelperDescriptors class.
        /// </summary>
        /// <param name="typeResolver"></param>
        /// <param name="viewComponentDescriptorProvider"></param>
        /// <param name="compilationService"></param>
        public ViewComponentTagHelpersDescriptorResolver(
            TagHelperTypeResolver typeResolver,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
            :base( false )
        {
            _viewComponentTagHelperDescriptorProvider = new ViewComponentTagHelperDescriptorProvider(viewComponentDescriptorProvider, compilationService);
            _compilationService = compilationService;
        }

        IEnumerable<TagHelperDescriptor> ITagHelperDescriptorResolver.Resolve(TagHelperDescriptorResolutionContext resolutionContext)
        {
            var descriptors = base.Resolve(resolutionContext);

            if (_viewComponentTagHelpersDescriptors == null)
            {
                _viewComponentTagHelpersDescriptors = ResolveViewComponentTagHelpersDescriptors(descriptors.FirstOrDefault()?.Prefix ?? string.Empty);
            }

            return descriptors.Concat(_viewComponentTagHelpersDescriptors);
        }

        private IEnumerable<TagHelperDescriptor> ResolveViewComponentTagHelpersDescriptors(string prefix)
        {
            var viewComponentTagHelperDescriptors = _viewComponentTagHelperDescriptorProvider.GetViewComponentTagHelperDescriptors();

            //Use the tagHelperDescriptorFactory to create descriptors for each viewCOmponentTagHelperDescriptor.
            TagHelperDescriptorFactory tagHelperDescriptorFactory = new TagHelperDescriptorFactory(false);
            IEnumerable<TagHelperDescriptor> resolvedDescriptors = new List<TagHelperDescriptor>();

            foreach (ViewComponentTagHelperDescriptor viewComponentTagHelperDescriptor in viewComponentTagHelperDescriptors)
            {
                var type = viewComponentTagHelperDescriptor.tagHelperType;
                var assemblyName = type.GetTypeInfo().Assembly.GetName().Name;
                var errorSink = new ErrorSink();
                var curDescriptors = tagHelperDescriptorFactory.CreateDescriptors(assemblyName, type, errorSink);

                resolvedDescriptors = resolvedDescriptors.Union(curDescriptors);
            }

            return resolvedDescriptors;
        }
    }
}
