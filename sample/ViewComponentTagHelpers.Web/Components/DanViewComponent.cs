using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ViewComponentTagHelpers
{
    public class DanViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string color)
        {
            this.GetType().GetTypeInfo();
            return View("Dan", new { Color = color });
        }
    }
}
