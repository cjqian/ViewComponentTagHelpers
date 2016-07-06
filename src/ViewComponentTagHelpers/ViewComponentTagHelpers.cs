using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ViewComponentTagHelpers
{
    [HtmlTargetElement("Custom")]
    public class ViewComponentTagHelpers : ITagHelper
    {
        //Ok since we're making it target element "Custom"
        //We might as well go balls deep and make a conversion list
        private Dictionary<string, int> _typeList;
        private readonly IViewComponentHelper _component;

        public ViewComponentTagHelpers(IViewComponentHelper component)
        {
            _component = component;
            SetTypeList();
        }

        //Yooooo this maps names of arguments in the view component
        //to its type
        private void SetTypeList()
        {
            _typeList = new Dictionary<string, int>
            {
                { "count", HtmlStringConverter.INT_TYPE },
                { "extraValue", HtmlStringConverter.STRING_TYPE }
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

            //var parameters = new object[context.AllAttributes.Count];
            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < context.AllAttributes.Count; i++)
            {
                var curAttribute = context.AllAttributes[i];

                //we just ignore if this doens't exist
                if (_typeList.ContainsKey(curAttribute.Name))
                {
                    var curValue = HtmlStringConverter.ConvertValue(curAttribute.Value, _typeList[curAttribute.Name]);
                    parameters[curAttribute.Name] = curValue;
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

        //CJQ added this to run
        public void Init(TagHelperContext context)
        {

        }
    }
}