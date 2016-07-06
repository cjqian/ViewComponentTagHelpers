using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.ComponentModel;

namespace ViewComponentTagHelpers
{
    [HtmlTargetElement("Custom")]
    [TypeConverter(typeof(HtmlStringConverter))]
    public class ViewComponentTagHelpers : ITagHelper
    {
        //Ok since we're making it target element "Custom"
        //We might as well go balls deep and make a conversion list
        private Dictionary<string, Type> _typeList;

        private readonly object[] _values = new object[10];
        private readonly IViewComponentHelper _component;
        private Type _viewComponentType;

        public ViewComponentTagHelpers(IViewComponentHelper component)
        {
            _component = component;
            SetTypeList();
        }

        //Yooooo this maps names of arguments in the view component
        //to its type
        private void SetTypeList()
        {
            _typeList = new Dictionary<string, Type>
            {
                { "count", typeof(int)},
                { "extraValue", typeof(string)}
            };
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public int Order
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public async void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((DefaultViewComponentHelper)_component).Contextualize(ViewContext);

            //var arguments = new Dictionary<string, object>();
            //for (var i = 0; i < _parametersProvided; i++)
            //{
            //    var cur = context.AllAttributes[i];
            //    arguments[cur.Name] = cur.Value;
            //}
            //dynamic arguments = new ExpandoObject();
            //for (var i = 0; i < _parametersProvided; i++)
            //{

            //    //var cur = context.AllAttributes[i];
            //    //arguments[cur.Name] = cur.Value;
            //}

            //var tmp = _component.GetType();
            //var tmp2 = _component.GetType().GetTypeInfo();
            //var tmp3 = _component.GetType().FullName;
            //var tmp4 = _component.GetType().GetMethods();
            //Why this and not invoke??
            //var vctpe = Type.GetType("CustomViewComponent");
           // MethodInfo method = typeof(CustomViewComponent);
            //var parmeters = method.GetParameters();
                       //var parameters = new object[context.AllAttributes.Count];
            var parameters = new Dictionary<string, object>();
            HtmlStringConverter converter = new HtmlStringConverter();

            for (var i = 0; i < context.AllAttributes.Count; i++)
            {
                var curName = context.AllAttributes[i].Name;

                //we just ignore if this doens't exist
                if (_typeList.ContainsKey(curName))
                {
                    Type curType = _typeList[curName];
                    string curValue = context.AllAttributes[i].Value.ToString();

                    if (converter.CanConvertTo(curType))
                    {
                        parameters[curName] = converter.ConvertTo(curValue, curType);
                    }
                }
            }

                      //var componentResult = await _component.InvokeAsync(output.TagName, new { Count = parameters[0], ExtraValue = parameters[1] } );
            var componentResult = await _component.InvokeAsync(output.TagName, parameters);
            //var tmp = new { count = 4, extraValue = "From view." };
            //var componentResult = await _component.InvokeAsync("Custom", arguments);
            //var componentResult = await _component.InvokeAsync("Custom", new { count = 4, extraValue = "From view." });

            output.SuppressOutput();
            output.Content.SetHtmlContent(componentResult);
        }

#pragma warning disable 1998
        public virtual async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Process(context, output);
        }

        //CJQ added this to run
        public void Init(TagHelperContext context)
        {

        }
#pragma warning restore 1998
    }
}