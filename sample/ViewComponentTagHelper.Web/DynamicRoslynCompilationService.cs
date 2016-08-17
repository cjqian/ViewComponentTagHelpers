using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
            ApplicationPartManager partManager,
            IOptions<RazorViewEngineOptions> optionsAccessor,
            IRazorViewEngineFileProviderAccessor fileProviderAccessor,
            ILoggerFactory loggerFactory
            )
                : base(
                      partManager,
                      optionsAccessor,
                      fileProviderAccessor,
                      loggerFactory
                      )
        {
        }

        CompilationResult ICompilationService.Compile(RelativeFileInfo fileInfo,
    string compilationContent)
        {
            return base.Compile(fileInfo, compilationContent);
        }
    }
}
