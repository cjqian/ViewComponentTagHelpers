using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

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

        public string WriteTagHelper(string viewComponentName)
        {
            ModifyAttribute("HtmlTargetElement", viewComponentName);
            ModifyClass(viewComponentName);

            //write to file
            /*
            var fileInfo = new RelativeFileInfo()

            var tmp = new DefaultRoslynCompilationService.
            */
            string newFile = viewComponentName + "ViewComponentTagHelpers.cs";
            return newFile;
        }

        private void ModifyAttribute(string attibuteName, string viewComponentName)
        {
            int lineNumber = FindLine(attibuteName);

            if (lineNumber == -1)
            {
                throw new Exception(attibuteName + " not found in template " + _templateFile);
            }

            string lineContent = MakeAttribute(attibuteName, viewComponentName);
            _lines[lineNumber] = "\t" + lineContent;
        }

        private void ModifyClass(string viewComponentName)
        {
            string namespaceName = "ViewComponentTagHelpers";

            //change the header
            string classHeader = "public class ";
            int lineNumber = FindLine(classHeader);

            if (lineNumber == -1)
            {
                throw new Exception("Invalid template file. Does not contain class.");
            }

            string className = viewComponentName + namespaceName;
            string lineContent = classHeader + className + " : ITagHelper";
            _lines[lineNumber] = "\t" + lineContent;

            //then, change the constructor
            string constructorHeader = "public " + namespaceName;
            lineNumber = FindLine(constructorHeader);

            if (lineNumber == -1)
            {
                throw new Exception("No constructor found.");
            }

            lineContent = "public " + className + "(IViewComponentHelper component)";
            _lines[lineNumber] = "\t" + lineContent;

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

        private string MakeAttribute(string attributeName, string attributeValue)
        {
            string attributeString = "[" + attributeName + "(\"" + attributeValue + "\")" + "]";
            return attributeString;
        }
    }
}
