using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

//namespace ViewComponentTagHelper.Web.Components
namespace ViewComponentTagHelper.Web
{
    public class AboutViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string email, String phoneNumber, int badgeID)
        {
            this.GetType().GetTypeInfo();
            return View("About", new { email, phoneNumber, badgeID});
        }
    }
}
