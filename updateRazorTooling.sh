# Remove NuGet packages. 
echo "Remove NuGet packages. "
rm -rf C:/Users/t-crqian/.nuget/packages/.tools
rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Razor.Design
rm -rf C:/Users/t-crqian/.nuget/packages/Microsoft.AspNetCore.Razor.Tools

# Restore and pack Microsoft.AspNetCore.Razor.Design
echo "Restore and pack Microsoft.AspNetCore.Razor.Design. "
cd ../RazorTooling/src/Microsoft.AspNetCore.Razor.Design
dotnet restore
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

# Restore and pack Microsoft.AspNetCore.Razor.Tools
echo "Restore and pack Microsoft.AspNetCore.Razor.Tools. "
cd ../Microsoft.AspNetCore.Razor.Tools
dotnet restore
dotnet pack -o C:/Users/t-crqian/Documents/LocalPackages

# Restore and build from sample
echo "Restore and build from sample. "
cd ../../../ViewComponentTagHelper/sample/ViewComponentTagHelper.Web/
dotnet restore
dotnet build

# dotnet razor-tooling resolve-taghelpers ./project.json Microsoft.AspNetCore.Mvc.TagHelpers

