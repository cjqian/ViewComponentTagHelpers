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
        //private static readonly Type ViewComponentTagHelperType = typeof(ViewComponentTagHelpers);
        private readonly ViewComponentTagHelperDescriptorProvider _viewComponentTagHelperDescriptorProvider;
        private IEnumerable<TagHelperDescriptor> _viewComponentTagHelpersDescriptors;
        private ICompilationService _compilationService;

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

            //currently, only runtime
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
