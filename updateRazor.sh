rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Razor
rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Razor.Runtime

# Build and move Razor.
echo "Building Razor."
cd ../Razor/src/Microsoft.AspNetCore.Razor
rm -rf bin/
rm -rf obj/
dotnet restore
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"

# Build and move Razor.
echo "Building Razor."
cd ../Microsoft.AspNetCore.Razor.Runtime
rm -rf bin/
rm -rf obj/
dotnet restore
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.Runtime.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\Web\Razor\v4.0"
cp bin/Debug/net451/Microsoft.AspNetCore.Razor.Runtime.dll "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Crystal"


