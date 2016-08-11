/*
#pragma checksum "/Views/Home/Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5f289973268e207010d34034c9caf4ae3694404b"
namespace AspNetCore
{
#line 1 "/Views/_ViewImports.cshtml"
    using Microsoft.AspNetCore.Mvc

#line default
#line hidden
    ;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System.Threading.Tasks;

    public class _Views_Home_Index_cshtml : Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
#line hidden
#pragma warning disable 0414
        private string __tagHelperStringValueBuffer = null;
#pragma warning restore 0414
        // type Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext
        // name __tagHelperExecutionContext
        // value 
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext = null;
        // type Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner
        // name __tagHelperRunner
        // value 
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = null;
        // type Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager
        // name __tagHelperScopeManager
        // value 
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager = null;
        // type Microsoft.AspNetCore.Mvc.TagHelpers.EnvironmentTagHelper
        // name __Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper
        // value 
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.EnvironmentTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper = null;
        // type ViewComponentTagHelper.Web.DanRothViewComponentTagHelper
        // name __ViewComponentTagHelper_Web_DanRothViewComponentTagHelper
        // value 
        private ViewComponentTagHelper.Web.DanRothViewComponentTagHelper __ViewComponentTagHelper_Web_DanRothViewComponentTagHelper = null;
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("jacket-color", "green", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#line hidden
        public _Views_Home_Index_cshtml()
        {
        }
#line hidden
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }

#line hidden

#pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            __tagHelperRunner = __tagHelperRunner ?? new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
            __tagHelperScopeManager = __tagHelperScopeManager ?? new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("environment", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "30933fbd9984425cb3aa5ecc08098ded", async () => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.EnvironmentTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            BeginContext(0, 27, false);
            Write(__tagHelperExecutionContext.Output);
            EndContext();
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            BeginContext(27, 48, true);
            WriteLiteral("\r\n<vc:about email=\"asdf\" phone-number=\"asdf\"/>\r\n");
            EndContext();
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("vc:dan-roth", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "a0b9dfbe66594229a90bcce402e422f0", async () => {
            }
            );
            __ViewComponentTagHelper_Web_DanRothViewComponentTagHelper = CreateTagHelper<global::ViewComponentTagHelper.Web.DanRothViewComponentTagHelper>();
            __tagHelperExecutionContext.Add(__ViewComponentTagHelper_Web_DanRothViewComponentTagHelper);
            __ViewComponentTagHelper_Web_DanRothViewComponentTagHelper.jacketColor = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            BeginContext(75, 35, false);
            Write(__tagHelperExecutionContext.Output);
            EndContext();
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            BeginContext(110, 28, true);
            WriteLiteral("\r\n<vc:derek badge=\"badge1\"/>");
            EndContext();
        }
#pragma warning restore 1998
        // HELLO, WORLD!
        [Microsoft.AspNetCore.Razor.TagHelpers.HtmlTargetElement("vc:dan-roth", TagStructure = Microsoft.AspNetCore.Razor.TagHelpers.TagStructure.NormalOrSelfClosing)]
        private class DanRothViewComponentTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
        {
            private readonly IViewComponentHelper _viewComponentHelper;
            public DanRothViewComponentTagHelper(IViewComponentHelper viewComponentHelper)
            {
                _viewComponentHelper = viewComponentHelper;
            }

            [ViewContext]
            public ViewContext ViewContext { get; set; }
            public System.String jacketColor { get; set; }

            public override async Task ProcessAsync(Microsoft.AspNetCore.Razor.TagHelpers.TagHelperContext context, Microsoft.AspNetCore.Razor.TagHelpers.TagHelperOutput output)
            {
                ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
                var viewContent = await _viewComponentHelper.InvokeAsync("DanRoth", new { jacketColor });
                output.TagName = null;
                output.Content.SetHtmlContent(viewContent);
            }
        }
    }
}*/
