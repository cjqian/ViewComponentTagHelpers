using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeResolver : TagHelperTypeResolver
    {
        private readonly ICompilationService _compilationService;
        private readonly ViewComponentTagHelperTypeProvider _viewComponentTagHelperTypeProvider;

        public ViewComponentTagHelperTypeResolver(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService
            ) : base()
        {
            _compilationService = compilationService;
            _viewComponentTagHelperTypeProvider = new ViewComponentTagHelperTypeProvider(
                viewComponentDescriptorProvider,
                compilationService);
        }

        protected override IEnumerable<TypeInfo> GetExportedTypes(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            IEnumerable<TypeInfo> results = base.GetExportedTypes(assemblyName);

            var tagHelperTypes = _viewComponentTagHelperTypeProvider.GetTagHelperTypes(assemblyName);
            if (tagHelperTypes != null)
            {
                foreach (var tagHelperType in tagHelperTypes)
                {
                        results = results.Append(tagHelperType);
                }
            }

            return results;
        }

        private bool ContainsType(IEnumerable<TypeInfo> types, string typeName)
        {
            foreach (var type in types)
            {
                if (type.Name.Equals(typeName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
