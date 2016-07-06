using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace ViewComponentTagHelpers
{
    public class HtmlStringConverter
    {
        public static readonly int INT_TYPE = 0;
        public static readonly int STRING_TYPE = 1;

        public static object ConvertValue(object value, int type)
        {
            string sValue = value.ToString();

            switch (type)
            {
                //INT_TYPE
                case 0:
                    return int.Parse(sValue);
                    break; 
                //STRING_TYPE
                case 1:
                    return sValue;
                default:
                    return sValue;
            }
        }
    }
}
