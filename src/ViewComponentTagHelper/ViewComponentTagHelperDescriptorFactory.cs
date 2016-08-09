using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperDescriptorFactory
    {
        private IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private bool _hasDescriptorProvider;

        public ViewComponentTagHelperDescriptorFactory(IViewComponentDescriptorProvider viewComponentDescriptorProvider)
        {
            _hasDescriptorProvider = true;
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
        }

        public ViewComponentTagHelperDescriptorFactory()
        {
            _hasDescriptorProvider = false;
        }

        // Resolves view component descriptors.
        public IEnumerable<TagHelperDescriptor> ResolveDescriptorsInAssembly(string assemblyName)
        {
            var tagHelperDescriptors = new List<TagHelperDescriptor>();

            var viewComponentDescriptors = ResolveViewComponentsInAssembly(assemblyName);
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                var tagHelperDescriptor = CreateTagHelperDescriptor(viewComponentDescriptor);
                tagHelperDescriptors.Add(tagHelperDescriptor);
            }

            return tagHelperDescriptors;
        }

        private IEnumerable<ViewComponentDescriptor> ResolveViewComponentsInAssembly(string assemblyName)
        {
            if (!_hasDescriptorProvider)
            {
                var descriptorProvider = CreateDescriptorProvider(assemblyName);
                var defaultViewComponents = descriptorProvider.GetViewComponents();
                return defaultViewComponents;
            }

            // We choose only view components in the given assembly.
            var viewComponents = new List<ViewComponentDescriptor>();
            var providedViewComponents = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponent in providedViewComponents)
            {
                var currentAssemblyName = GetAssemblyName(viewComponent);
                if (currentAssemblyName.Equals(assemblyName))
                {
                    viewComponents.Add(viewComponent);
                }
            }

            return viewComponents;
        }

        private IViewComponentDescriptorProvider CreateDescriptorProvider(string assemblyName)
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            var partManager = new ApplicationPartManager();
            partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            partManager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            // TODO: Allow customization by user. 
            var viewComponentDescriptorProvider = new DefaultViewComponentDescriptorProvider(partManager);
            return viewComponentDescriptorProvider;
        }

        private TagHelperDescriptor CreateTagHelperDescriptor(ViewComponentDescriptor viewComponentDescriptor)
        {
            IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors;
            IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors;

            if (!TryGetAttributeDescriptors(viewComponentDescriptor,
                out attributeDescriptors,
                out requiredAttributeDescriptors))
            {
                throw new Exception("Something went wrong.");
            }

            var tagName = GetLowerKebab(viewComponentDescriptor.ShortName);
            var tagHelperDescriptor = new TagHelperDescriptor
            {
                // CR: String interpolation.
                TagName = FormatTagName(viewComponentDescriptor),

                // CR: TypeName too generic. __Generated__DanViewComponentTagHelper;
                TypeName = FormatTypeName(viewComponentDescriptor),
                AssemblyName = GetAssemblyName(viewComponentDescriptor),
                Attributes = attributeDescriptors,
                RequiredAttributes = requiredAttributeDescriptors,
                TagStructure = TagStructure.NormalOrSelfClosing
            };

            return tagHelperDescriptor;
        }

        private string GetAssemblyName(ViewComponentDescriptor viewComponentDescriptor)
        {
            var fullName = viewComponentDescriptor.TypeInfo.Assembly.FullName;

            // Notice that the full name is split by commas.
            var assemblyName = fullName.Split(',')[0];
            return assemblyName;
        }
        private string FormatTagName(ViewComponentDescriptor viewComponentDescriptor) =>
            $"vc:{GetLowerKebab(viewComponentDescriptor.ShortName)}";

        private string FormatTypeName(ViewComponentDescriptor viewComponentDescriptor) =>
            $"{viewComponentDescriptor.DisplayName}TagHelper";

        // CR: Match method names of TagHelperDescriptorFactory
        // TODO: Add support to HtmlTargetElement, HtmlAttributeName (vc: asdfadf)

        // CR: Expose lower kebab in razor.
        // CR: Move tag helper descriptor creation into crystal.lib
        // CR: Add validation of view component; valid attribute names?
        private bool TryGetAttributeDescriptors(
            ViewComponentDescriptor viewComponentDescriptor,
            out IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors,
            out IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors
            )
        {
            var methodParameters = viewComponentDescriptor.MethodInfo.GetParameters();
            var descriptors = new List<TagHelperAttributeDescriptor>();
            var requiredDescriptors = new List<TagHelperRequiredAttributeDescriptor>();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];
                var lowerKebabName = GetLowerKebab(parameter.Name);
                var tagHelperAttributeDescriptor = new TagHelperAttributeDescriptor
                {
                    Name = lowerKebabName,
                    PropertyName = parameter.Name,
                    TypeName = parameter.ParameterType.FullName
                };

                // CR: typeof(string.FullName) ?? CHeck
                if (tagHelperAttributeDescriptor.TypeName == "System.String"
                    || tagHelperAttributeDescriptor.TypeName.Equals("String"))
                {
                    tagHelperAttributeDescriptor.IsStringProperty = true;
                }

                descriptors.Add(tagHelperAttributeDescriptor);

                if (!parameter.HasDefaultValue)
                {
                    var requiredAttributeDescriptor = new TagHelperRequiredAttributeDescriptor
                    {
                        Name = lowerKebabName
                    };
                    requiredDescriptors.Add(requiredAttributeDescriptor);
                }
            }

            attributeDescriptors = descriptors;
            requiredAttributeDescriptors = requiredDescriptors;

            return true;
        }

        // CR: Remove and expose.
        // TagHelperDescriptorFactory returns this lower-kebab attribute as a full tag name.
        // However, this is called after the file is returned.
        private string GetLowerKebab(string word)
        {
            if (word.Length == 0) return "";

            var stringBuilder = new StringBuilder();
            var wordArray = word.ToCharArray();

            // If capitalized and not the first character, will replace with dash and lower case.
            stringBuilder.Append(Char.ToLower(wordArray[0]));
            for (var i = 1; i < wordArray.Length; i++)
            {
                var character = wordArray[i];
                if (Char.IsUpper(character))
                {
                    stringBuilder.Append("-" + Char.ToLower(character));
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
