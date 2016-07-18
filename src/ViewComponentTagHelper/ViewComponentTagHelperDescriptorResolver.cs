// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperDescriptorResolver : ITagHelperDescriptorResolver
    {
        private readonly ViewComponentTagHelperTypeProvider _viewComponentTagHelperTypeProvider;
        private readonly ICompilationService _compilationService;
        private IEnumerable<TagHelperDescriptor> _ViewComponentTagHelperDescriptors;
        private ITagHelperDescriptorResolver _tagHelperDescriptorResolver;

        // CR: Transition class into TagHelperTypeResolver.
        public ViewComponentTagHelperDescriptorResolver(
            ITagHelperDescriptorResolver descriptorResolver,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
        {
            _compilationService = compilationService;
            _tagHelperDescriptorResolver = descriptorResolver;
            _viewComponentTagHelperTypeProvider = new ViewComponentTagHelperTypeProvider(
                viewComponentDescriptorProvider,
                compilationService);
        }

        IEnumerable<TagHelperDescriptor> ITagHelperDescriptorResolver.Resolve(
            TagHelperDescriptorResolutionContext resolutionContext)
        {
            var descriptors = _tagHelperDescriptorResolver.Resolve(resolutionContext);
            if (_ViewComponentTagHelperDescriptors == null)
            {
                _ViewComponentTagHelperDescriptors = ResolveViewComponentTagHelperDescriptors(
                    descriptors.FirstOrDefault()?.Prefix ?? string.Empty, resolutionContext.ErrorSink);
            }

            return descriptors.Concat(_ViewComponentTagHelperDescriptors);
        }

        private IEnumerable<TagHelperDescriptor> ResolveViewComponentTagHelperDescriptors(string prefix, ErrorSink errorSink)
        {
            var resolvedDescriptors = new List<TagHelperDescriptor>();

            // Use the tagHelperDescriptorFactory to create descriptors for each viewComponentTagHelperDescriptor.
            var tagHelperDescriptorFactory = new TagHelperDescriptorFactory(false);

            var tagHelperTypes = _viewComponentTagHelperTypeProvider.GetTagHelperTypes();
            foreach (var tagHelperType in tagHelperTypes)
            {
                var assemblyName = tagHelperType.GetTypeInfo().Assembly.GetName().Name;
                var resolvedDescriptor = tagHelperDescriptorFactory.CreateDescriptors(assemblyName,
                    tagHelperType, errorSink);
                resolvedDescriptors.AddRange(resolvedDescriptor);
            }

            return resolvedDescriptors;
        }
    }
}
