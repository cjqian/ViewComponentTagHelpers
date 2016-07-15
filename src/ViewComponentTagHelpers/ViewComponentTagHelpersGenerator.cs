// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelpersGenerator
    {
        private readonly string[] _lines;

        public ViewComponentTagHelpersGenerator()
        {
            // Yes, this is over 120 characters, but hopefully we will someday not need the whole path.
            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var templateFile = "ViewComponentTagHelpersTemplate.txt";
            _lines = System.IO.File.ReadAllLines(rootDirectory + templateFile);
        }

        public string WriteTagHelper(ViewComponentDescriptor viewComponentDescriptor)
        {
            //Set variables where they're used
            string[] lines = (string[])_lines.Clone();

            var viewComponentName = viewComponentDescriptor.ShortName;
            string lowerKebab = GetLowerKebab(viewComponentName);
            lines = FindAndReplace(lines, "[[HtmlTargetElement]]", lowerKebab);

            lines = FindAndReplace(lines, "[[ViewComponentName]]", viewComponentName);

            var viewComponentType = viewComponentDescriptor.TypeInfo;
            lines = SetParameters(lines, viewComponentType);

            return LinesToString(lines);
        }
        
        // CR: figure out razor (not mine);; taghelperdescriptorfactory
        private string GetLowerKebab(string word)
        {
            //TODO: make sure camel/pascal case? Will only ever be referenced internally.
            if (word.Length == 0) return "";

            StringBuilder sb = new StringBuilder();
            char[] wordArray = word.ToCharArray();

            //If capitalized and not the first character, will replace with dash and lower case.
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
            //Sets global parameters. 
            var getSet = "{ get; set; }";

            //Get the invoked method of the view component.
            // CR: Call the way the MVC calls this.
            var invokableMethod = viewComponentType.GetMethods().Where(info => info.Name.Equals("Invoke") || info.Name.Equals("InvokeAsync")).FirstOrDefault();
            if (invokableMethod == null)
            {
                // TODO: Make this a resource
                throw new Exception("This view component has no invokable method.");
            }

            // CR: check how MVC does this
            var methodParameters = invokableMethod.GetParameters();
            var methodParameterStrings = new string[methodParameters.Length];

            StringBuilder sb = new StringBuilder();

            //Each parameter gets a new declaration.
            for (var i = 0; i < methodParameters.Length; i++)
            {
                //set the methodParameterStrings
                var parameter = methodParameters[i];

                // CR: use String interpolation (adsf)
                // CR: combine space and get set
                methodParameterStrings[i] = "    public " + parameter.ParameterType.Name + " " + parameter.Name + " " + getSet;

                //and add to object
                sb.Append(parameter.Name);

                //do this separately (the object part)
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
            for (var i = 0; i < lines.Length; i++)
            {

                    lines[i] = lines[i].Replace(before, after);
            }

            return lines;
        }


        private string[] FindAndReplace(string[] lines, string before, string[] after)
        {
            var line = FindLine(before);
            if (line == -1)
            {
                //Use string interpolation
                throw new Exception(before + " is not found in the template.");
            }

            // CR: Normalize string.format and then gooooooo
            // CR: CHeck out string.format
            var firstPart = new string[line];
            Array.Copy(lines, 0, firstPart, 0, firstPart.Length);

            //Plus/minus one because we don't want the original [[]] placeholder.
            var secondPart = new string[lines.Length - line - 1];
            Array.Copy(lines, line + 1, secondPart, 0, secondPart.Length);

            string[] sumParts = firstPart.Concat(after).ToArray().Concat(secondPart).ToArray();
            return sumParts;
        }


        private int FindLine(string keyword)
        {
            for (var i = 0; i < _lines.Length; i++)
            {
                //Returns the first line containing the keyword.
                if (_lines[i].Contains(keyword))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}