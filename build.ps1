$ErrorActionPreference = "Stop"
./build/dotnet-install.ps1 -JsonFile ./global.json
dotnet run --project build/Build.csproj -- $args
exit $LASTEXITCODE;