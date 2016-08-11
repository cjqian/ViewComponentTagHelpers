./updateRazor.sh
./updateMvc.sh
./updateRazorTooling.sh

cd sample/ViewComponentTagHelper.Web
dotnet restore
dotnet build
