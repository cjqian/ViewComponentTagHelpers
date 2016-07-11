﻿using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using System;
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

        public ViewComponentTagHelpersDescriptorResolver(
            TagHelperTypeResolver typeResolver,
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
            :base( false )
        {
            _viewComponentTagHelperDescriptorProvider = new ViewComponentTagHelperDescriptorProvider(viewComponentDescriptorProvider, compilationService);
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
            //var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            var viewComponentTagHelperDescriptors = _viewComponentTagHelperDescriptorProvider.GetViewComponentTagHelperDescriptors();
            //Here's where we generate the code for tag helpers
            /*
            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var rootFile = "ViewComponentTagHelpersTemplate.txt";
            ViewComponentTagHelpersGenerator generator = new ViewComponentTagHelpersGenerator(rootDirectory, rootFile);
            generator.WriteTagHelper("Custom");
            */
            var resolvedDescriptors = new List<TagHelperDescriptor>();

            foreach (var viewComponentTagHelperDescriptor in viewComponentTagHelperDescriptors)
            {
                IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors;
                var viewComponentDescriptor = viewComponentTagHelperDescriptor.viewComponentDescriptor;
                var viewComponentType = viewComponentTagHelperDescriptor.viewComponentType;
                if (TryGetViewComponentAttributeDescriptors(viewComponentDescriptor.GetType(), out attributeDescriptors))
                {
                    //make descriptors out of all strings, and turn this into requiredAttributeList
                    var attributeDescriptorNames = attributeDescriptors.Select(descriptor => descriptor.Name);
                    List<TagHelperRequiredAttributeDescriptor> requiredAttributeList = new List<TagHelperRequiredAttributeDescriptor>();

                    foreach (var attributeDescriptorName in attributeDescriptorNames)
                    {
                        var curDescriptor = new TagHelperRequiredAttributeDescriptor();
                        curDescriptor.Name = attributeDescriptorName;

                        requiredAttributeList.Add(curDescriptor);
                    }

                    resolvedDescriptors.Add(
                        new TagHelperDescriptor
                        {
                            Prefix = prefix,
                            TagName = viewComponentDescriptor.ShortName,
                            TypeName = viewComponentType.FullName,
                            AssemblyName = viewComponentType.GetTypeInfo().Assembly.GetName().Name,
                            Attributes = attributeDescriptors,
                            RequiredAttributes = requiredAttributeList
                        });
                }
            }
            /*
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors;
                if (TryGetViewComponentAttributeDescriptors(viewComponentDescriptor.GetType(), out attributeDescriptors))
                {
                    //make descriptors out of all strings, and turn this into requiredAttributeList
                    var attributeDescriptorNames = attributeDescriptors.Select(descriptor => descriptor.Name);
                    List<TagHelperRequiredAttributeDescriptor> requiredAttributeList = new List<TagHelperRequiredAttributeDescriptor>();

                    foreach (var attributeDescriptorName in attributeDescriptorNames)
                    {
                        var curDescriptor = new TagHelperRequiredAttributeDescriptor();
                        curDescriptor.Name = attributeDescriptorName;

                        requiredAttributeList.Add(curDescriptor);
                    }

                    resolvedDescriptors.Add(
                        new TagHelperDescriptor
                        {
                            Prefix = prefix,
                            TagName = viewComponentDescriptor.ShortName,
                            TypeName = ViewComponentTagHelperType.FullName,
                            AssemblyName = ViewComponentTagHelperType.GetTypeInfo().Assembly.GetName().Name,
                            Attributes = attributeDescriptors,
                            RequiredAttributes = requiredAttributeList
                        });
                }
            }*/

            return resolvedDescriptors;
        }

        private bool TryGetViewComponentAttributeDescriptors(Type type, out IEnumerable<TagHelperAttributeDescriptor> attributeDescriptors)
        {
            var methods = type.GetMethods();
            var invocableMethod = methods.Where(info => info.Name.StartsWith("Invoke", StringComparison.Ordinal)).FirstOrDefault();

            if (invocableMethod == null)
            {
                attributeDescriptors = null;
                return false;
            }

            var methodParameters = invocableMethod.GetParameters();
            var descriptors = new List<TagHelperAttributeDescriptor>();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];

                descriptors.Add(
                    new TagHelperAttributeDescriptor
                    {
                        Name = parameter.Name,
                        PropertyName = "Parameter" + i,
                        TypeName = parameter.ParameterType.FullName
                    });
            }

            attributeDescriptors = descriptors;

            return true;
        }
    }
}
