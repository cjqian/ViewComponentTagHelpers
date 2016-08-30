using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

//namespace ViewComponentTagHelper.Web.Components
namespace ViewComponentTagHelper.Web
{
    public class AboutViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string email, String phoneNumber, int badgeID=5)
        {
            return View("About", new { email, phoneNumber, badgeID});
        }
    }
}
