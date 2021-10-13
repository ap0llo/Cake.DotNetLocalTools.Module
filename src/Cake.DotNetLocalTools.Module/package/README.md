# Cake.DotNetLocalTools.Module

A [Cake](https://cakebuild.net/) Module that extends Cake with functionality to install tools from a .NET tool manifest.

You can read the latest documentation at https://github.com/ap0llo/Cake.DotNetLocalTools.Module


## Table of Contents

- [Overview](#overview)
- [Usage](#usage)
- [Example](#example)
- [License](#license)


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

## License

Cake.DotNetLocalTools.Module is licensed under the MIT License.

For details see https://github.com/ap0llo/Cake.DotNetLocalTools.Module/blob/master/LICENSE