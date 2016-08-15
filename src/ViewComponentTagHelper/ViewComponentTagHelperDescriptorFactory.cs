using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperDescriptorFactory : ITagHelperDescriptorFactory
    {
        private IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private bool _hasDescriptorProvider;

        // Question: should factories inherently take nothing in the constructor and have all static things?
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

        // Just so this is of type ITagHelperDescriptorFactory..
        public IEnumerable<TagHelperDescriptor> CreateDescriptors(string assemblyName, Type type, ErrorSink errorSink)
        {
            var tagHelperDescriptors = ResolveDescriptorsInAssembly(assemblyName);
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
            // Set attributes.
            IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors;
            IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors;
            Dictionary<string, object> requiredAttributeValues;

            if (!TryGetAttributeDescriptors(viewComponentDescriptor,
                out attributeDescriptors,
                out requiredAttributeDescriptors,
                out requiredAttributeValues))
            {
                throw new Exception("Something went wrong.");
            }

            // Because this is a view component, we want to add to the property bag.
            // CR: Do we need all of the items in this property bag?
            var propertyBag = new Dictionary<string, object>();
            propertyBag["ViewComponentShortName"] = viewComponentDescriptor.ShortName;
            propertyBag["ViewComponentTagHelperTypeName"] = $"__Generated__{viewComponentDescriptor.TypeInfo.Name}TagHelper";
            propertyBag["ViewComponentDefaultValues"] = requiredAttributeValues;

            var tagName = TagHelperDescriptorFactory.ToHtmlCase(viewComponentDescriptor.ShortName);

            var tagHelperDescriptor = new TagHelperDescriptor
            {
                TagName = FormatTagName(viewComponentDescriptor),
                TypeName = FormatTypeName(viewComponentDescriptor),
                AssemblyName = GetAssemblyName(viewComponentDescriptor),
                Attributes = attributeDescriptors,
                RequiredAttributes = requiredAttributeDescriptors,
                TagStructure = TagStructure.NormalOrSelfClosing,
                PropertyBag = propertyBag
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
            $"vc:{TagHelperDescriptorFactory.ToHtmlCase(viewComponentDescriptor.ShortName)}";

        private string FormatTypeName(ViewComponentDescriptor viewComponentDescriptor) =>
            $"{viewComponentDescriptor.DisplayName}TagHelper";

        // TODO: Add support to HtmlTargetElement, HtmlAttributeName (vc: asdfadf)
        // TODO: Add validation of view component; valid attribute names?
        private bool TryGetAttributeDescriptors(
            ViewComponentDescriptor viewComponentDescriptor,
            out IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors,
            out IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors,
            out Dictionary<string, object> requiredAttributeValues
            )
        {
            var methodParameters = viewComponentDescriptor.MethodInfo.GetParameters();
            var descriptors = new List<TagHelperAttributeDescriptor>();
            var requiredDescriptors = new List<TagHelperRequiredAttributeDescriptor>();
            var requiredValues = new Dictionary<string, object>();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];
                var lowerKebabName = TagHelperDescriptorFactory.ToHtmlCase(parameter.Name);
                var tagHelperAttributeDescriptor = new TagHelperAttributeDescriptor
                {
                    Name = lowerKebabName,
                    PropertyName = parameter.Name,
                    TypeName = parameter.ParameterType.FullName
                };

                var tagHelperType = Type.GetType(tagHelperAttributeDescriptor.TypeName);
                if (tagHelperType.Equals(typeof(string)))
                {
                    tagHelperAttributeDescriptor.IsStringProperty = true;
                }

                descriptors.Add(tagHelperAttributeDescriptor);

                if (parameter.HasDefaultValue)
                {
                    requiredValues[lowerKebabName] = parameter.DefaultValue;
                }
                else
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
            requiredAttributeValues = requiredValues;

            return true;
        }
    }
}
