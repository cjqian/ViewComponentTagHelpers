using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperDescriptorResolver : TagHelperDescriptorResolver
    {
        private readonly Dictionary<string, List<ViewComponentDescriptor>> _viewComponentDictionary;

        public ViewComponentTagHelperDescriptorResolver(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider)
            : base(false)
        {
            _viewComponentDictionary = new Dictionary<string, List<ViewComponentDescriptor>>();
            var viewComponents = viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponent in viewComponents)
            {
                var assemblyName = viewComponent.TypeInfo.Namespace;
                if (!_viewComponentDictionary.ContainsKey(assemblyName))
                {
                    _viewComponentDictionary[assemblyName] = new List<ViewComponentDescriptor>();
                }

                _viewComponentDictionary[assemblyName].Add(viewComponent);
            }
        }

        protected override IEnumerable<TagHelperDescriptor> ResolveDescriptorsInAssembly(string assemblyName, SourceLocation documentLocation, ErrorSink errorSink)
        {
            var tagHelperDescriptors = base.ResolveDescriptorsInAssembly(assemblyName, documentLocation, errorSink);

            if (!_viewComponentDictionary.ContainsKey(assemblyName))
            {
                return tagHelperDescriptors;
            }

            var viewComponentDescriptors = ResolveViewComponentDescriptorsInAssembly(assemblyName);
            return tagHelperDescriptors.Concat(viewComponentDescriptors);
        }

        private IEnumerable<TagHelperDescriptor> ResolveViewComponentDescriptorsInAssembly(string assemblyName)
        {
            var tagHelperDescriptors = new List<TagHelperDescriptor>();
            foreach (var viewComponent in _viewComponentDictionary[assemblyName])
            {
                var tagHelperDescriptor = ResolveViewComponentDescriptor(viewComponent);
                tagHelperDescriptors.Add(tagHelperDescriptor);
            }

            return tagHelperDescriptors;
        }

        private TagHelperDescriptor ResolveViewComponentDescriptor(ViewComponentDescriptor viewComponent)
        {
            var tagHelperDescriptor = new TagHelperDescriptor();

            tagHelperDescriptor.AssemblyName = viewComponent.TypeInfo.Namespace;
            tagHelperDescriptor.Attributes = ResolveViewComponentAttributes(viewComponent);
            tagHelperDescriptor.TagName = FormatTagName(viewComponent);
            tagHelperDescriptor.TagStructure = Microsoft.AspNetCore.Razor.TagHelpers.TagStructure.NormalOrSelfClosing;
            tagHelperDescriptor.TypeName = FormatTypeName(viewComponent);
            return tagHelperDescriptor;
        }

        // Returns something like "vc:tag-name"
        private string FormatTagName(ViewComponentDescriptor viewComponent)
        {
            var tagName = "vc:" + GetLowerKebab(viewComponent.ShortName);
            return tagName;
        }

        // Returns something like "ViewComponentTagHelper.Web.AboutViewComponentTagHelper"
        private string FormatTypeName(ViewComponentDescriptor viewComponent)
        {
            var typeName = viewComponent.DisplayName + "TagHelper";
            return typeName;
        }

        private IEnumerable<TagHelperAttributeDescriptor> ResolveViewComponentAttributes(ViewComponentDescriptor viewComponent)
        {
            var tagHelperAttributeDescriptors = new List<TagHelperAttributeDescriptor>();
            // This list needs a view context attribute.
            tagHelperAttributeDescriptors.Add(GetViewContextAttribute());

            var viewComponentParameters = viewComponent.MethodInfo.GetParameters();
            foreach (var viewComponentParameter in viewComponentParameters)
            {
                var tagHelperAttributeDescriptor = ResolveViewComponentAttribute(viewComponentParameter);
                tagHelperAttributeDescriptors.Add(tagHelperAttributeDescriptor);
            }
            
            return tagHelperAttributeDescriptors;
        }

        private TagHelperAttributeDescriptor ResolveViewComponentAttribute(ParameterInfo viewComponentParameter)
        {
            var tagHelperAttributeDescriptor = new TagHelperAttributeDescriptor();

            tagHelperAttributeDescriptor.Name = GetLowerKebab(viewComponentParameter.Name);
            tagHelperAttributeDescriptor.PropertyName = viewComponentParameter.Name;
            tagHelperAttributeDescriptor.TypeName = viewComponentParameter.ParameterType.Name;

            return tagHelperAttributeDescriptor;
        }

        private TagHelperAttributeDescriptor GetViewContextAttribute()
        {
            var viewContextAttribute = new TagHelperAttributeDescriptor();
            viewContextAttribute.Name = "view-context";
            viewContextAttribute.PropertyName = "ViewContext";
            viewContextAttribute.TypeName = "Microsoft.AspNetCore.Mvc.Rendering.ViewContext";

            return viewContextAttribute;
        }

        // TODO: Refer to a better version of this.
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
