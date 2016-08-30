rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Mvc.Razor.Host
rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Mvc.Razor

# Razor
cd ../Mvc/src/Microsoft.AspNetCore.Mvc.Razor/
rm -rf bin
rm -rf obj
dotnet restore
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Razor.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Razor.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Core
cd ../Microsoft.AspNetCore.Mvc.Core/
rm -rf bin
rm -rf obj
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Core.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Core.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Razor Host
cd ../Microsoft.AspNetCore.Mvc.Razor.Host/
rm -rf bin
rm -rf obj
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Razor.Host.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Razor.Host.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"


# Formatters.Json
cd ../Microsoft.AspNetCore.Mvc.Formatters.Json/
rm -rf bin
rm -rf obj
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Formatters.Json.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Formatters.Json.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Abstractions
cd ../Microsoft.AspNetCore.Mvc.Abstractions/
rm -rf bin
rm -rf obj
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Abstractions.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.Abstractions.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Data Annotations
cd ../Microsoft.AspNetCore.Mvc.DataAnnotations/
rm -rf bin 
rm -rf obj
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.DataAnnotations.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Mvc.DataAnnotations.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"


