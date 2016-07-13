using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

//namespace ViewComponentTagHelpers.Web.Components
namespace ViewComponentTagHelpers
{
    public class AboutViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string email, string phoneNumber, string cat, string pranav)
        {
            this.GetType().GetTypeInfo();
            return View("About", new { email, phoneNumber, cat, pranav });
        }
    }
}
