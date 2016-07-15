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
        private readonly ICompilationService _compilationService;
        private IEnumerable<TagHelperDescriptor> _viewComponentTagHelpersDescriptors;

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
            // CR: No inherit from tgtyperesovler
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
            //CR: Just need types, keep list (revert to ViewCOmponentDescirpotrs)
            var tagHelperTypes = _viewComponentTagHelperDescriptorProvider.GetTagHelperTypes();

            //Use the tagHelperDescriptorFactory to create descriptors for each viewCOmponentTagHelperDescriptor.
            //CR: Use vars
            var tagHelperDescriptorFactory = new TagHelperDescriptorFactory(false);
            IEnumerable<TagHelperDescriptor> resolvedDescriptors = new List<TagHelperDescriptor>();

            foreach (var tagHelperType in tagHelperTypes)
            {
                var assemblyName = tagHelperType.GetTypeInfo().Assembly.GetName().Name;
                //CR: Pass error sink from Resolve 
                var errorSink = new ErrorSink();
                //CR: IEnumerable
                //CR: No abbreviations
                var curDescriptors = tagHelperDescriptorFactory.CreateDescriptors(assemblyName, tagHelperType, errorSink);
                
                // I changed from union
                resolvedDescriptors = resolvedDescriptors.Concat(curDescriptors);
            }

            return resolvedDescriptors;
        }
    }
}
