using System;
using System.Collections.Generic;
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

        // Q: should factories inherently take nothing in the constructor and have all static things?
        public ViewComponentTagHelperDescriptorFactory(IViewComponentDescriptorProvider viewComponentDescriptorProvider)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
        }

        // Note: We don't need the type or the errorSink, but we need the following method call for this descriptor factory
        // to be of type ITagHelperDescriptorFactory.
        public IEnumerable<TagHelperDescriptor> CreateDescriptors(string assemblyName, Type type, ErrorSink errorSink)
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
            // We choose only view components in the given assembly.
            var viewComponents = new List<ViewComponentDescriptor>();
            var providedViewComponents = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponent in providedViewComponents)
            {
                var currentAssemblyName = viewComponent.TypeInfo.Assembly.GetName().Name;
                if (currentAssemblyName.Equals(assemblyName))
                {
                    viewComponents.Add(viewComponent);
                }
            }

            return viewComponents;
        }

        private TagHelperDescriptor CreateTagHelperDescriptor(ViewComponentDescriptor viewComponentDescriptor)
        {
            // Set attributes.
            IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors;
            IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors;

            if (!TryGetAttributeDescriptors(viewComponentDescriptor,
                out attributeDescriptors,
                out requiredAttributeDescriptors))
            {
                throw new Exception("Something went wrong.");
            }

            // Because this is a view component, we want to add to the property bag.
            var propertyBag = new Dictionary<string, string>();
            propertyBag["ViewComponentShortName"] = viewComponentDescriptor.ShortName;
            propertyBag["ViewComponentTagHelperTypeName"] = $"__Generated__{viewComponentDescriptor.TypeInfo.Name}TagHelper";

            var tagName = TagHelperDescriptorFactory.ToHtmlCase(viewComponentDescriptor.ShortName);

            var tagHelperDescriptor = new TagHelperDescriptor
            {
                TagName = FormatTagName(viewComponentDescriptor),
                TypeName = FormatTypeName(viewComponentDescriptor),
                AssemblyName = viewComponentDescriptor.TypeInfo.Assembly.GetName().Name,
                Attributes = attributeDescriptors,
                RequiredAttributes = requiredAttributeDescriptors,
                TagStructure = TagStructure.NormalOrSelfClosing,
                PropertyBag = propertyBag
            };

            return tagHelperDescriptor;
        }

        private string FormatTagName(ViewComponentDescriptor viewComponentDescriptor) =>
            $"vc:{TagHelperDescriptorFactory.ToHtmlCase(viewComponentDescriptor.ShortName)}";

        private string FormatTypeName(ViewComponentDescriptor viewComponentDescriptor) =>
            $"__Generated__{viewComponentDescriptor.DisplayName}TagHelper";

        // TODO: Add support to HtmlTargetElement, HtmlAttributeName (vc: asdfadf)
        // TODO: Add validation of view component; valid attribute names?
        private bool TryGetAttributeDescriptors(
            ViewComponentDescriptor viewComponentDescriptor,
            out IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors,
            out IEnumerable<TagHelperRequiredAttributeDescriptor> requiredAttributeDescriptors
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
    }
}
