using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ViewComponentTagHelpers
{
    //Needs to be of type String
    public class ObjectConverter : TypeConverter
    {
        private List<Type> _convertibles = new List<Type> { typeof(int), typeof(string) };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            //Everything is an object!
            if (sourceType == typeof(object))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (_convertibles.Contains(destinationType))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null) return "";
                return value.ToString();
            }
            
            if (destinationType == typeof(int))
            {
                if (value == null) return -1;
                return int.Parse(value.ToString());
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
