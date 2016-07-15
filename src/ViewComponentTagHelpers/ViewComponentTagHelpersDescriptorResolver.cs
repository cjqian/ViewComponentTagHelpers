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
    public class ViewComponentTagHelpersDescriptorResolver : TagHelperDescriptorResolver, ITagHelperDescriptorResolver
    {
        private readonly ViewComponentTagHelperDescriptorProvider _viewComponentTagHelperDescriptorProvider;
        private readonly ICompilationService _compilationService;
        private IEnumerable<TagHelperDescriptor> _viewComponentTagHelpersDescriptors;

        public ViewComponentTagHelpersDescriptorResolver(
            TagHelperTypeResolver typeResolver,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
            :base( false )
        {
            // CR: No inherit from tagtyperesovler?? 
            _viewComponentTagHelperDescriptorProvider = new ViewComponentTagHelperDescriptorProvider(
                viewComponentDescriptorProvider, compilationService );
            _compilationService = compilationService;
        }

        IEnumerable<TagHelperDescriptor> ITagHelperDescriptorResolver.Resolve(
            TagHelperDescriptorResolutionContext resolutionContext )
        {
            var descriptors = base.Resolve(resolutionContext);
            if (_viewComponentTagHelpersDescriptors == null)
            {
                _viewComponentTagHelpersDescriptors = ResolveViewComponentTagHelpersDescriptors(
                    descriptors.FirstOrDefault()?.Prefix ?? string.Empty, resolutionContext.ErrorSink);
            }

            return descriptors.Concat(_viewComponentTagHelpersDescriptors);
        }

        private IEnumerable<TagHelperDescriptor> ResolveViewComponentTagHelpersDescriptors(string prefix, ErrorSink errorSink)
        {
            // Can't make this a var, or can't concat in loop.
            IEnumerable<TagHelperDescriptor> resolvedDescriptors = new List<TagHelperDescriptor>();

            // Use the tagHelperDescriptorFactory to create descriptors for each viewComponentTagHelperDescriptor.
            var tagHelperDescriptorFactory = new TagHelperDescriptorFactory(false);

            var tagHelperTypes = _viewComponentTagHelperDescriptorProvider.GetTagHelperTypes();
            foreach (var tagHelperType in tagHelperTypes)
            {
                var assemblyName = tagHelperType.GetTypeInfo().Assembly.GetName().Name;
                var resolvedDescriptor = tagHelperDescriptorFactory.CreateDescriptors(assemblyName, 
                    tagHelperType, errorSink);
                resolvedDescriptors = resolvedDescriptors.Concat(resolvedDescriptor);
            }

            return resolvedDescriptors;
        }
    }
}
