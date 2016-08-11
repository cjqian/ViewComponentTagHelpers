rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Mvc.Razor.Host

cd ../Mvc/src/Microsoft.AspNetCore.Mvc.Razor.Host/
dotnet restore
dotnet build
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages


