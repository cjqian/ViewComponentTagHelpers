using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewComponentTagHelper
{
    public class ViewComponentTagHelperTypeWrapper
    {
        public IEnumerable<Type> Types;
        public IEnumerable<string> Namespaces;

        public ViewComponentTagHelperTypeWrapper(IEnumerable<Type> types, IEnumerable<string> namespaces)
        {
            Types = types;
            Namespaces = namespaces;
        }
    }
}
