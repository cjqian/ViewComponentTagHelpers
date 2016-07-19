// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeProvider
    {
        private readonly IViewComponentDescriptorProvider _viewComponentDescriptorProvider;
        private readonly ViewComponentTagHelperGenerator _ViewComponentTagHelperGenerator;
        private readonly DynamicRosylnCompilationService _compilationService;

        private readonly Dictionary<string, Type> _compiledTagHelperTypesByName;
        private readonly Dictionary<string, List<TypeInfo>> _compiledTagHelperTypesByAssembly;

        public ViewComponentTagHelperTypeProvider(
            IViewComponentDescriptorProvider viewComponentDescriptorProvider,
            ICompilationService compilationService)
        {
            _viewComponentDescriptorProvider = viewComponentDescriptorProvider;
            _compilationService = (DynamicRosylnCompilationService)compilationService;

            _compiledTagHelperTypesByName = new Dictionary<string, Type>();
            _compiledTagHelperTypesByAssembly = new Dictionary<string, List<TypeInfo>>();

            // TODO: put all classes together so compile/make references once?
            // TODO: embed or write out individual template

            _ViewComponentTagHelperGenerator = new ViewComponentTagHelperGenerator();
        }

        public IList<TypeInfo> GetTagHelperTypes(AssemblyName assembly)
        {
            UpdateTagHelperTypes();

            List<TypeInfo> typeList;
            if (_compiledTagHelperTypesByAssembly.TryGetValue(assembly.Name, out typeList))
            {
                return typeList;
            };

            return null;
        }

        // Creates a tag helper for each view component and caches. 
        private void UpdateTagHelperTypes()
        {
            var viewComponentDescriptors = _viewComponentDescriptorProvider.GetViewComponents();
            foreach (var viewComponentDescriptor in viewComponentDescriptors)
            {
                if (!_compiledTagHelperTypesByName.ContainsKey(viewComponentDescriptor.ShortName))
                {
                    var fileInfo = new DummyFileInfo();
                    var relativeFileInfo = new RelativeFileInfo(fileInfo, "./");

                    // Generates a tagHelperFile (string .cs tag helper equivalent of the tag helper.)
                    var tagHelperFile = _ViewComponentTagHelperGenerator.WriteTagHelper(viewComponentDescriptor);
                    var compilationResult = _compilationService.AddReferenceAndCompile(relativeFileInfo, tagHelperFile);

                    // Cache. 
                    var type = compilationResult.CompiledType;

                    _compiledTagHelperTypesByName[viewComponentDescriptor.ShortName] = type;

                    // If this is a new namespace, we create a new list.
                    // We add the type to its associated assemblies.

                    List<TypeInfo> typeList;
                    if (!_compiledTagHelperTypesByAssembly.TryGetValue(type.Namespace, out typeList))
                    {
                        typeList = new List<TypeInfo>();
                        _compiledTagHelperTypesByAssembly[type.Namespace] = typeList;
                    }

                    typeList.Add(type.GetTypeInfo());
                }

            }
        }
    }
}