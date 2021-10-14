# Cake.DotNetLocalTools.Module

[![NuGet](https://img.shields.io/nuget/v/Cake.DotNetLocalTools.Module.svg?logo=nuget)](https://www.nuget.org/packages/Cake.DotNetLocalTools.Module)
[![Azure Artifacts](https://img.shields.io/badge/Azure%20Artifacts-prerelease-yellow.svg?logo=azuredevops)](https://dev.azure.com/ap0llo/OSS/_packaging?_a=feed&feed=PublicCI)

[![Build Status](https://dev.azure.com/ap0llo/OSS/_apis/build/status/Cake.DotNetLocalTools.Module?branchName=master)](https://dev.azure.com/ap0llo/OSS/_build/latest?definitionId=21&branchName=master)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-green.svg)](https://conventionalcommits.org)

A [Cake](https://cakebuild.net/) Module that extends Cake with functionality to install tools from a .NET tool manifest.

## Overview

Cake allows installing [.NET CLI Tools](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) through the `#tool` preprocessor directies and Cake.Frosting's `InstallTool` method (see [Installing and using tools](https://cakebuild.net/docs/writing-builds/tools/installing-tools) for details).

.NET Core 3.1 introduced the concept of ["local tools"](https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use).
Local tools are listed in a "tool manifest" (`dotnet-tools.json`) and run through the `dotnet` command.

`Cake.DotNetLocalTools.Module` brings these two concepts together. 
It reads a list of tools from one or more tool manifests and install's them through Cake's tool infrastructure.
This way, you can easily use the tools from Cake while still having the tools and their versions described in a common format.

## Usage

### Cake Script

To use the module in a Cake script file, perform the following steps

1. Add the preprocessor directive to install the module

    ```cs
    #module nuget:?package=Cake.DotNetLocalTools.Module&version=VERSION
    ```

1. Install tools using the `#tool` preprocessor directive and a uri scheme of `toolmanifest`:

    ```cs
    #tool "toolmanifest:?package=.config/dotnet-tools.json"
    ```

### Cake.Frosting

To use the module in a [Cake.Frosting](https://cakebuild.net/docs/running-builds/runners/cake-frosting) project, perform the following steps.

1. Add the [Azure Artifacts feed](https://dev.azure.com/ap0llo/OSS/_packaging?_a=feed&feed=Cake.DotNetLocalTools.Module) to your `nuget.config` (the module is not yet available on NuGet.org):
    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
    <packageSources>
        ...
        <add key="Cake.DotNetLocalTools.Module" value="https://pkgs.dev.azure.com/ap0llo/OSS/_packaging/PublicCI/nuget/v3/index.json" />
    </packageSources>
    </configuration>
    ```

1. Install the module package by adding a package reference to your project

    ```xml
    <PackageReference Include="Cake.DotNetLocalTools.Module" Version="VERSION" /> 
    ```

1. Register `LocalToolsModule` with the Cake Host:

    ```cs
    using Cake.DotNetLocalTools.Module;

    public static int Main(string[] args)
    {
        return new CakeHost()
            // Register LocalToolsModule
            .UseModule<LocalToolsModule>()        
            // Continue with the standard setup of a Cake.Frosting project
            .UseContext<BuildContext>()
            .Run(args);
    }
    ```

1. You can not install tools using either the `InstallToolsFromManifest()` method or using `InstallTools()` with a uri scheme of `toolmanifest`:

    ```cs
    using Cake.DotNetLocalTools.Module;

    public static int Main(string[] args)
    {
        return new CakeHost()
            // Register LocalToolsModule
            .UseModule<LocalToolsModule>() 
            // Install Tools (both install options are equivalent)
            .InstallToolsFromManifest(".config/dotnet-tools.json")       
            .InstallTools(new Uri("tool-manifest:?package=.config/dotnet-tools.json"))       
            // Continue with the standard setup of a Cake.Frosting project
            .UseContext<BuildContext>()
            .Run(args);
    }
    ```

## Example

Installing tools from a tool manifest is equivalent to installing the tools listed in the manifest using a `#tool` preprocessor directive or through `InstallTool()`.

For example, a tool manifest at `.config/dotnet-tools.json` could look like this:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "nbgv": {
      "version": "3.4.231",
      "commands": [
        "nbgv"
      ]
    },
    "dotnet-format": {
      "version": "5.1.225507",
      "commands": [
        "dotnet-format"
      ]
    },
    "dotnet-reportgenerator-globaltool": {
      "version": "4.8.12",
      "commands": [
        "reportgenerator"
      ]
    }
  }
}
```

Tools from the manifest can be installed using 

```cs
#tool "toolmanifest:?package=.config/dotnet-tools.json"
```

This is equivalent to installing each tool individually:


```cs
#tool "dotnet:?package=nbgv&version=3.4.231"
#tool "dotnet:?package=dotnet-format&version=5.1.225507"
#tool "dotnet:?package=dotnet-reportgenerator-globaltool&version=4.8.12"
```


## Building from source

Building the project from source requires the .NET 5 SDK (version 5.0.400 as specified in [global.json](./global.json)).

To build the project, run 

```ps1
.\build.ps1
```

This will 

- Download the required version of the .NET SDK
- Build the project
- Run all tests 
- Pack the NuGet package.


### Versioning and Branching

The version of the library is automatically derived from git and the information
in `version.json` using [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning):

- The master branch  always contains the latest version. Packages produced from
  master are always marked as pre-release versions (using the `-pre` suffix).
- Stable versions are built from release branches. Build from release branches
  will have no `-pre` suffix
- Builds from any other branch will have both the `-pre` prerelease tag and the git
  commit hash included in the version string

To create a new release branch use the [`nbgv` tool](https://www.nuget.org/packages/nbgv/):

```ps1
dotnet tool install --global nbgv
nbgv prepare-release
```
