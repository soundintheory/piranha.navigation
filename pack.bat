dotnet restore
dotnet clean
dotnet build -c Release

dotnet pack modules/SoundInTheory.Piranha.Navigation.Links/SoundInTheory.Piranha.Navigation.Links.csproj --no-build -c Release -o ./.nuget
dotnet pack modules/SoundInTheory.Piranha.Navigation.Menus/SoundInTheory.Piranha.Navigation.Menus.csproj --no-build -c Release -o ./.nuget