// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{
    public class DynamicRosylnCompilationService : DefaultRoslynCompilationService, ICompilationService
    {
        private ReferenceManager _referenceManager;

        public DynamicRosylnCompilationService(
            ApplicationPartManager partManager, 
            IOptions<RazorViewEngineOptions> optionsAccessor, 
            IRazorViewEngineFileProviderAccessor fileProviderAccessor, 
            ILoggerFactory loggerFactory,
            ReferenceManager referenceManager) 
                : base(
                      partManager, 
                      optionsAccessor, 
                      fileProviderAccessor, 
                      loggerFactory
                      )
        {
            _referenceManager = referenceManager;
        }

        protected override IList<MetadataReference> GetCompilationReferences()
        {
            return _referenceManager.GetReferences();
        }
    }
}
