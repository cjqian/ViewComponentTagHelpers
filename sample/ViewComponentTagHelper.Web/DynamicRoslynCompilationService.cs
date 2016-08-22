using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper.Web
{
    public class DynamicRosylnCompilationService : DefaultRoslynCompilationService, ICompilationService
    {
        public DynamicRosylnCompilationService(
            CSharpCompiler compiler,
            IRazorViewEngineFileProviderAccessor fileProviderAccessor,
            IOptions<RazorViewEngineOptions> optionsAccessor,
            ILoggerFactory loggerFactory
            ) : base(compiler, fileProviderAccessor, optionsAccessor, loggerFactory) 
        {
        }

        CompilationResult ICompilationService.Compile(RelativeFileInfo fileInfo,
    string compilationContent)
        {
            return base.Compile(fileInfo, compilationContent);
        }
    }
}
