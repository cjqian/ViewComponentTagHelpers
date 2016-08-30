using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace ViewComponentTagHelper.Web
{
    public class DanRothViewComponent : ViewComponent  
    {
        private string _asciiTextString = @"C:\Users\t-crqian\Documents\visual studio 2015\Projects\ViewComponentTagHelper\sample\ViewComponentTagHelper.Web\wwwroot\DanRoth.txt";

        public IViewComponentResult Invoke(string jacketColor)
        {
            this.GetType().GetTypeInfo();


            // mark all as star
            IEnumerable<char> jacketColors = new List<char> { 'N', 'D', '8', '0', 'M' };
            string[] ascii_text = System.IO.File.ReadAllLines(_asciiTextString);
            // jacket
            for (int i = 100; i < ascii_text.Length; i++)
            {
                foreach (char c in ascii_text[i])
                {
                    if (jacketColors.Contains(c))
                    {
                        ascii_text[i] = ascii_text[i].Replace(c, '#');
                    }
                }
            }

            // replace all stars
            string replacement = getSpan(jacketColor);
            for (int i = 0; i < ascii_text.Length; i++)
            {
                ascii_text[i] = ascii_text[i].Replace("#", replacement);
            }

            dynamic obj = new ExpandoObject();
            obj.List = ascii_text.ToList();
            return View("DanRoth", obj);
        }

        private string getSpan(string color)
        {
            string before = "<span style='color:" + color + "'>";
            string middle = "#";
            string end = "</span>";

            string all = before + middle + end;
            return all;
        }
    }
}
