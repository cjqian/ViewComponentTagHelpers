// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewComponentTagHelpers
{
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
