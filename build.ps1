$ErrorActionPreference = "Stop"
./build/dotnet-install.ps1 -Channel 6.0 -Runtime dotnet # .NET 6 runtime is required for the Grynwald.ChangeLog tool. Remove once it is updated to a version that runs on .NET 7
./build/dotnet-install.ps1 -JsonFile ./global.json
dotnet run --project build/Build.csproj -- $args
exit $LASTEXITCODE;