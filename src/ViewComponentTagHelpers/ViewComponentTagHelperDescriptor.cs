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
