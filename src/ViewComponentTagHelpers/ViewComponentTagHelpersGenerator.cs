using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelpersGenerator
    {
        private string _rootDirectory;
        private string _templateFile;
        private string[] _lines;

        public ViewComponentTagHelpersGenerator(string rootDirectory, string templateFile)
        {
            _rootDirectory = rootDirectory;
            _templateFile = templateFile;

            _lines = System.IO.File.ReadAllLines(_rootDirectory + _templateFile);
        }

        public string WriteTagHelper(ViewComponentDescriptor viewComponentDescriptor)
        {
            var viewComponentName = viewComponentDescriptor.ShortName;
            var viewComponentType = viewComponentDescriptor.TypeInfo;

            string[] lines = (string[])_lines.Clone();

            //HtmlTargetElement
            string lowerKebab = GetLowerKebab(viewComponentName);
            lines = FindAndReplace(lines, "[[HtmlTargetElement]]", lowerKebab);
            //ViewComponentName
            lines = FindAndReplace(lines, "[[ViewComponentName]]", viewComponentName);

            //CustomParameters, ParametersObject
            lines = SetParameters(lines, viewComponentType);

            return LinesToString(lines);
        }
        
        private string GetLowerKebab(string word)
        {
            if (word.Length == 0) return "";

            StringBuilder sb = new StringBuilder();
            char[] wordArray = word.ToCharArray();

            sb.Append(Char.ToLower(wordArray[0]));
            for (int i = 1; i < wordArray.Length; i++)
            {
                char cur = wordArray[i];
                if (Char.IsUpper(cur))
                {
                    sb.Append("-" + Char.ToLower(cur));
                } else
                {
                    sb.Append(cur);
                }
            }

            return sb.ToString();
        }

        private string[] SetParameters(string[] lines, TypeInfo viewComponentType)
        {
            //First, set global parameters
            //Default ref
            var getSet = "{ get; set; }";

            //Get the invoked method of the view component.
            var invokableMethod = viewComponentType.GetMethods().Where(info => info.Name.StartsWith("Invoke", StringComparison.Ordinal)).FirstOrDefault();
            if (invokableMethod == null)
            {
                throw new Exception("This view component has no invokable method.");
            }

            var methodParameters = invokableMethod.GetParameters();
            var methodParameterStrings = new string[methodParameters.Length];

            //Then, make a new object
            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                //set the methodParameterStrings
                var parameter = methodParameters[i];
                methodParameterStrings[i] = "\tpublic " + parameter.ParameterType.Name + " " + parameter.Name + " " + getSet;

                //and add to object
                sb.Append(parameter.Name);

                if (i < methodParameters.Length - 1)
                {
                    sb.Append(",");
                }
            }

            //Replace global parameters 
            lines = FindAndReplace(lines, "[[CustomParameters]]", methodParameterStrings);

            //Replace [[ParametersObject]]
            lines = FindAndReplace(lines, "[[ParametersObject]]", sb.ToString());

            return lines;
        }

        private string LinesToString(string[] lines)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private string[] FindAndReplace(string[] lines, string before, string after)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(before))
                {
                    lines[i] = lines[i].Replace(before, after);
                }
            }

            return lines;
        }

        private string[] FindAndReplace(string[] lines, string before, string[] after)
        {
            var line = FindLine(before);
            if (line == -1)
            {
                throw new Exception(before + " is not found in the template.");
            }

            var firstPart = new string[line];
            Array.Copy(lines, 0, firstPart, 0, firstPart.Length);

            //Plus/minus one because we don't want the original [[]] placeholder.
            var secondPart = new string[lines.Length - line - 1];
            Array.Copy(lines, line + 1, secondPart, 0, secondPart.Length);

            string[] sumParts = firstPart.Concat(after).ToArray().Concat(secondPart).ToArray();
            return sumParts;
        }

        //finds the line containing the first instance of the keyword, or -1 if none is found
        private int FindLine(string keyword)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                if (_lines[i].Contains(keyword))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}