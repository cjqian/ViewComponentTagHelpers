using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
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
            ) : base ()
        {
            _compilationService = compilationService;
            _viewComponentTagHelperTypeProvider = new ViewComponentTagHelperTypeProvider(
                viewComponentDescriptorProvider,
                compilationService);
        }

        protected override IEnumerable<TypeInfo> GetExportedTypes(AssemblyName assemblyName) 
        {
            // BREAKPOINT: verify assemblyName
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            IEnumerable<TypeInfo> results = new List<TypeInfo>();

            var tagHelperTypeWrappers = _viewComponentTagHelperTypeProvider.GetTagHelperTypeWrappers();
            if (tagHelperTypeWrappers.Namespaces.Contains(assemblyName.Name))
            {
                foreach (var tagHelperType in tagHelperTypeWrappers.Types)
                {
                    if (tagHelperType.Namespace.Equals(assemblyName.Name) 
                        && !ContainsType(results, tagHelperType.Name)
                       )
                    {
                        results = results.Append(tagHelperType.GetTypeInfo());
                    }
                }
            }

            results = results.Concat(base.GetExportedTypes(assemblyName));
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

        private bool AssembliesEqual(AssemblyName x, AssemblyName y)
        {
            // Ignore case because that's what Assembly.Load does.
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.CultureName ?? string.Empty, y.CultureName ?? string.Empty, StringComparison.Ordinal);
        } 
    }
}
