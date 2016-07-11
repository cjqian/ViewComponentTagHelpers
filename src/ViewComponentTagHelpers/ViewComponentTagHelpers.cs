﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace ViewComponentTagHelpers
{
    [HtmlTargetElement("Custom")]
    [TypeConverter(typeof(ObjectConverter))]
    public class ViewComponentTagHelpers : ITagHelper
    {
        private readonly IViewComponentHelper _component;

        [HtmlAttributeName("view-component-type")]
        public Type ViewComponentType { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public void Init(TagHelperContext context)
        {

        }

        public ViewComponentTagHelpers(IViewComponentHelper component)
        {
            _component = component;
        }

        private Dictionary<string, Type> GetTypeList()
        {
            Dictionary<string, Type> typeList = new Dictionary<string, Type>();

            //TODO: Get first method that STARTS with Invoke (could be InvokeAsync)
            ParameterInfo[] parameters = ViewComponentType.GetMethod("Invoke").GetParameters();

            foreach (ParameterInfo parameter in parameters){
                typeList[parameter.Name] = parameter.ParameterType;
            }

            return typeList;
        }

        public int Order
        {
            get
            {
                return 1;
            }
        }

        public async void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((DefaultViewComponentHelper)_component).Contextualize(ViewContext);

            Dictionary<string, Type> typeList = GetTypeList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ObjectConverter converter = new ObjectConverter();

            for (var i = 0; i < context.AllAttributes.Count; i++)
            {
                var curName = context.AllAttributes[i].Name;

                if (typeList.ContainsKey(curName))
                {
                    Type curType = typeList[curName];
                    object curValue = context.AllAttributes[i].Value;

                    if (converter.CanConvertTo(curType))
                    {
                        parameters[curName] = converter.ConvertTo(curValue, curType);
                    } else
                    {
                        parameters[curName] = curValue;
                    }
                }
            }

            var componentResult = await _component.InvokeAsync(output.TagName, parameters);

            output.SuppressOutput();
            output.Content.SetHtmlContent(componentResult);
        }

#pragma warning disable 1998
        public virtual async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Process(context, output);
        }
#pragma warning restore 1998
    }
}