using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeResolver : TagHelperTypeResolver
    {
        private readonly ViewComponentTagHelperTypeProvider _viewComponentTagHelperTypeProvider;

        public ViewComponentTagHelperTypeResolver(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ViewComponentCompilationService viewComponentCompilationService,
            ReferenceManager referenceManager
            ) : base()
        {
            _viewComponentTagHelperTypeProvider = new ViewComponentTagHelperTypeProvider(
                viewComponentDescriptorProvider,
                viewComponentCompilationService,
                referenceManager
                );
        }

        protected override IEnumerable<TypeInfo> GetExportedTypes(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            var  results = base.GetExportedTypes(assemblyName);
            var tagHelperTypes = _viewComponentTagHelperTypeProvider.GetTagHelperTypes();
            foreach (var tagHelperType in tagHelperTypes)
            {
                if (tagHelperType.Namespace.Equals(assemblyName.Name))
                {
                    results = results.Append(tagHelperType);
                }
            }

            return results;
        }
    }
}
