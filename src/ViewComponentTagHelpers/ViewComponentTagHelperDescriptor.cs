// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;

namespace ViewComponentTagHelpers
{
    /// <summary>
    /// A wrapper containing a viewComponentDescriptor and a compiled type for its associated tag helper. 
    /// Necessary to dynamically compile each view component with a separate tag helper association. 
    /// </summary>
    public class ViewComponentTagHelperDescriptor
    {
        public readonly ViewComponentDescriptor viewComponentDescriptor;
        public readonly Type tagHelperType;

        public ViewComponentTagHelperDescriptor(ViewComponentDescriptor descriptor, Type type)
        {
            viewComponentDescriptor = descriptor;
            tagHelperType = type;
        }
    }
}
