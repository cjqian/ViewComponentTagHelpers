// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ViewComponentTagHelpers
{
    public class ViewComponentTagHelpersGenerator
    {
        private readonly string _lines;

        public ViewComponentTagHelpersGenerator()
        {
            // Yes, this is over 120 characters, but hopefully we will someday not need the whole path.
            var rootDirectory = "C:\\Users\\t-crqian\\Documents\\Visual Studio 2015\\Projects\\ViewComponentTagHelpers\\src\\ViewComponentTagHelpers\\";
            var templateFile = "ViewComponentTagHelpersTemplate.txt";
            _lines = TranslateForStringFormatting(System.IO.File.ReadAllLines(rootDirectory + templateFile));
        }

        private string TranslateForStringFormatting(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("{", "{{");
                lines[i] = lines[i].Replace("}", "}}");
                lines[i] = lines[i].Replace("[[", "{");
                lines[i] = lines[i].Replace("]]", "}"); 
            }
            return LinesToString(lines);
        }

        public string WriteTagHelper(ViewComponentDescriptor viewComponentDescriptor)
        {
            //Set variables where they're used
            var viewComponentName = viewComponentDescriptor.ShortName;
            var lowerKebab = GetLowerKebab(viewComponentName);
            var viewComponentType = viewComponentDescriptor.TypeInfo;


            var methodParameters = GetMethodParameters(viewComponentType);
            var parametersInitialized = GetInitializedParameters(methodParameters);
            var parametersObject = GetObjectParameters(methodParameters);

            string[] formattedParameters = new string[] { lowerKebab, viewComponentName, parametersInitialized, parametersObject };

            var formattedLines = String.Format(_lines, formattedParameters);
            return formattedLines;
        }

        //Razor's TagHelperDescriptorFactory does this in a private ToIndexerAttributeDescriptor method. Should we copy?
        private string GetLowerKebab(string word)
        {
            //TODO: Check numbers, symbols, etc. See above comment.
            if (word.Length == 0) return "";

            StringBuilder sb = new StringBuilder();
            char[] wordArray = word.ToCharArray();

            // If capitalized and not the first character, will replace with dash and lower case.
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
        
        private ParameterInfo[] GetMethodParameters(TypeInfo viewComponentType)
        {
            //CR: Call the way MVC calls this.
            var invokableMethod = viewComponentType.GetMethods().Where(info => info.Name.Equals("Invoke") || info.Name.Equals("InvokeAsync")).FirstOrDefault();
            if (invokableMethod == null)
            {
                // TODO: Make this a resource
                throw new Exception("This view component has no invokable method.");
            }

            var methodParameters = invokableMethod.GetParameters();
            return methodParameters;
        }

        private string GetInitializedParameters(ParameterInfo[] methodParameters)
        {
            var getSet = " {get; set; }";
           
            var methodParameterStrings = new string[methodParameters.Length];

            //Each parameter gets a new declaration.
            for (var i = 0; i < methodParameters.Length; i++)
            {
                //set the methodParameterStrings
                var parameter = methodParameters[i];

                methodParameterStrings[i] = "    public " + parameter.ParameterType.Name + " " + parameter.Name + getSet;
            }

            return LinesToString(methodParameterStrings);
        }

        private string GetObjectParameters(ParameterInfo[] methodParameters)
        {
            var methodParameterStrings = new string[methodParameters.Length];

            StringBuilder sb = new StringBuilder();

            //Each parameter gets a new declaration.
            for (var i = 0; i < methodParameters.Length; i++)
            {
                //set the methodParameterStrings
                var parameter = methodParameters[i];

                //and add to object
                sb.Append(parameter.Name);

                //do this separately (the object part)
                if (i < methodParameters.Length - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        //String.Concat does not add \n after each line.
        private string LinesToString(string[] lines)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}