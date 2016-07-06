using System;
using System.Collections.Generic;
using System.IO;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelpersGenerator
    {
        
        public void WriteFile()
        {
            string filePath = "./TagHelpers/ViewComponentTagHelpersX.cs";

            if (this.NeedToWrite(filePath))
            {
                List<string> lines = new List<string>();
                lines.Add("namespace ViewComponentTagHelpers {");
                lines.Add("public class ViewComponentTagHelpersX{");
                lines.Add("public static int TestGet(){");
                lines.Add("return 5;");
                lines.Add("}");
                lines.Add("}");
                lines.Add("}");

                //File.WriteAllLines("./TagHelpers/ViewComponentTagHelpersX.cs", lines.ToArray());
            }
        }

        private bool NeedToWrite(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return true;
            }

            var creationTime = File.GetCreationTimeUtc(filePath);
            var curTime = DateTime.UtcNow;

            //reset time is 30 seconds
            var resetTime = creationTime.AddSeconds(30);

            return (DateTime.Compare(resetTime, curTime) <= 0) ;
        }
    }

}
