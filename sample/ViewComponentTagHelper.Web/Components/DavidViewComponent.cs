using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

//namespace ViewComponentTagHelper.Web.Components
namespace ViewComponentTagHelper.Web
{
    public class DavidViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string money, string power)
        {
            this.GetType().GetTypeInfo();
            return View("David", new { money, power });
        }
    }
}
