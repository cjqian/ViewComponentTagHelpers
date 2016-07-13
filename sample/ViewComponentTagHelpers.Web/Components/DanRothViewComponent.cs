using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ViewComponentTagHelpers
{
    public class DanRothViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string color, bool water)
        {
            this.GetType().GetTypeInfo();
            return View("DanRoth", new { Color = color, Water = water });
        }
    }
}
