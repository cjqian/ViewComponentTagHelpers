using Microsoft.AspNetCore.Mvc;
using System.Reflection;

//This is a normal view component invocation.
namespace ViewComponentTagHelpers
{
    public class CustomViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int count, string extraValue)
        {
            this.GetType().GetTypeInfo();
            return View("Custom", new { Count = count, ExtraValue = extraValue });
        }
    }
}
