using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeResolver : TagHelperTypeResolver
    {
        private readonly ViewComponentTagHelperTypeProvider _viewComponentTagHelperTypeProvider;

        public ViewComponentTagHelperTypeResolver(
            ViewComponentTagHelperTypeProvider viewComponentTagHelperTypeProvider 
            ) : base()
        {
            _viewComponentTagHelperTypeProvider = viewComponentTagHelperTypeProvider;
        }

        protected override IEnumerable<TypeInfo> GetExportedTypes(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            // Get tag helper types of view components, appened to list of types for
            // this particular namespace (if appropriate), and return results.
            var results = base.GetExportedTypes(assemblyName).ToList();
            var tagHelperTypes = _viewComponentTagHelperTypeProvider.GetTagHelperTypes();
            foreach (var tagHelperType in tagHelperTypes)
            {
                if (tagHelperType.Namespace.Equals(assemblyName.Name))
                {
                    results.Add(tagHelperType);
                }
            }

            return results;
        }
    }
}
