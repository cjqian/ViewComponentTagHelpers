using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewComponentTagHelpers
{
    //Has a CompilationResult and also a CSharpCompilation
    public class CSharpCompilationResult
    {
        private CompilationResult _compilationResult;
        private CSharpCompilation _cSharpCompilation;

        public CSharpCompilationResult(CompilationResult compilationResult, CSharpCompilation cSharpCompilation)
        {
            _compilationResult = compilationResult;
            _cSharpCompilation = cSharpCompilation;
        }

        public CompilationResult getCompilationResult()
        {
            return _compilationResult;
        }

        public CSharpCompilation getCSharpCompilation()
        {
            return _cSharpCompilation;
        }

    }
}
