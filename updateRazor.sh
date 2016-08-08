crystalPath="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Build and move Razor.
echo "Building Razor."
cd ../Razor/src/Microsoft.AspNetCore.Razor
dotnet restore
dotnet build
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"

# Build and move Razor.
echo "Building Razor."
cd ../Microsoft.AspNetCore.Razor.Runtime
dotnet restore
dotnet build
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.Runtime.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"


