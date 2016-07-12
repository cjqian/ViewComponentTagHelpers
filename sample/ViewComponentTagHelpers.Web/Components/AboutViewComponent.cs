using Microsoft.AspNetCore.Mvc;
using System.Reflection;

//namespace ViewComponentTagHelpers.Web.Components
namespace ViewComponentTagHelpers
{
    public class AboutViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string email, string phoneNumber)
        {
            this.GetType().GetTypeInfo();
            return View("About", new { Email = email, PhoneNumber = phoneNumber });
        }
    }
}
