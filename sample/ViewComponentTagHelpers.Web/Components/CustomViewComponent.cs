using Microsoft.AspNetCore.Mvc;

//This is a normal view component invocation.
namespace ViewComponentTagHelpers
{
    public class CustomViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int count, string extraValue)
        {
            return View("Custom", new { Count = count, ExtraValue = extraValue });
        }
    }
}
