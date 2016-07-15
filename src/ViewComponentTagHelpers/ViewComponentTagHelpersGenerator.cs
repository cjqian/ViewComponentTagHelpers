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
            var viewComponentName = viewComponentDescriptor.ShortName;
            var lowerKebab = GetLowerKebab(viewComponentName);
            var methodParameters = viewComponentDescriptor.MethodInfo.GetParameters();
            var parametersInitialized = GetInitializedParameters(methodParameters);
            var parametersObject = GetObjectParameters(methodParameters);

            var formattedParameters = new string[] { lowerKebab, viewComponentName,
                parametersInitialized, parametersObject };

            var formattedLines = String.Format(_lines, formattedParameters);
            return formattedLines;
        }

        // TagHelperDescriptorFactory returns this lower-kebab attribute as a full tag name.
        // However, this is called after the file is returned.
        private string GetLowerKebab(string word)
        {
            if (word.Length == 0) return "";

            var stringBuilder = new StringBuilder();
            var wordArray = word.ToCharArray();

            // If capitalized and not the first character, will replace with dash and lower case.
            stringBuilder.Append(Char.ToLower(wordArray[0]));
            for (var i = 1; i < wordArray.Length; i++)
            {
                var character = wordArray[i];
                if (Char.IsUpper(character))
                {
                    stringBuilder.Append("-" + Char.ToLower(character));
                } else
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }
        
        // Returns "public foo {get; set:} \n public bar {get; set;} \n", etc.
        private string GetInitializedParameters(ParameterInfo[] methodParameters)
        {
            var getSet = " {get; set; }";
           
            var methodParameterStrings = new string[methodParameters.Length];

            // Each parameter gets a new declaration.
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];
                methodParameterStrings[i] = "public " + parameter.ParameterType.Name + " " + parameter.Name + getSet;
            }

            return LinesToString(methodParameterStrings);
        }

        // Returns "foo, bar", etc.
        private string GetObjectParameters(ParameterInfo[] methodParameters)
        {
            var methodParameterStrings = new string[methodParameters.Length];
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                stringBuilder.Append(methodParameters[i].Name);

                if (i < methodParameters.Length - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            return stringBuilder.ToString();
        }

        // String.Concat does not add \n after each line.
        private string LinesToString(string[] lines)
        {
            var stringBuilder = new StringBuilder();

            foreach (var line in lines)
            {
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }
    }
}