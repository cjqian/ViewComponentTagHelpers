// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ViewComponentTagHelpers
{
    /// <summary>
    /// Generates a tag helper file for a given view component.
    /// </summary>
    public class ViewComponentTagHelpersGenerator
    {
        private string _rootDirectory;
        private string _templateFile;
        private string[] _lines;

        /// <summary>
        /// Creates an instance of the ViewComponentTagHelpersGenerator class.
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <param name="templateFile"></param>
        public ViewComponentTagHelpersGenerator(string rootDirectory, string templateFile)
        {
            _rootDirectory = rootDirectory;
            _templateFile = templateFile;

            _lines = System.IO.File.ReadAllLines(_rootDirectory + _templateFile);
        }

        /// <summary>
        /// Given a viewComponentDescriptor, returns the C# code of an associated tag helper.
        /// </summary>
        /// <param name="viewComponentDescriptor"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Returns the lower kebab case alternative for a camel/pascal case word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a line declaring a property of each tag helper dependent on arguments of the Invoke() method of the view component.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="viewComponentType"></param>
        /// <returns></returns>
        private string[] SetParameters(string[] lines, TypeInfo viewComponentType)
        {
            //Sets global parameters. 
            var getSet = "{ get; set; }";

            //Get the invoked method of the view component.
            var invokableMethod = viewComponentType.GetMethods().Where(info => info.Name.StartsWith("Invoke", StringComparison.Ordinal)).FirstOrDefault();
            if (invokableMethod == null)
            {
                throw new Exception("This view component has no invokable method.");
            }

            var methodParameters = invokableMethod.GetParameters();
            var methodParameterStrings = new string[methodParameters.Length];

            StringBuilder sb = new StringBuilder();

            //Each parameter gets a new declaration.
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

        /// <summary>
        ///Converts a string[] into a long string with new lines separating each line.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private string LinesToString(string[] lines)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Replaces each instance of before in lines with after.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        private string[] FindAndReplace(string[] lines, string before, string after)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(before))
                {
                    lines[i] = lines[i].Replace(before, after);
                }
            }

            return lines;
        }

        /// <summary>
        /// Replace each instance of before in lines with after.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds the line containing the first instance of the keyword, or -1 if none is found.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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