![Macabre2D Logo](icon.png "Macabre2D Logo")

# Macabre2D

A game engine built on top of MonoGame. Features a cross platform editor created with AvaloniaUI.

## Building

### Supported Platforms

Macabre2D can be built and run on Windows (8/10/11) and Linux (Ubuntu).

### Git LFS

You must install Git LFS (Large File Storage) to use this repository. [This article](https://help.github.com/en/articles/installing-git-large-file-storage) should assist you.

### MonoGame Content Builder (MGCB)

You must install the MGCB dotnet tool to run the editor. Full instructions can be found [here](https://docs.monogame.net/articles/tools/mgcb.html); however, here is a brief explanation:

* Make sure the .NET Core SDK is installed
* In a terminal, run `dotnet tool install -g dotnet-mgcb`

### Building from Source

The full source code is available here from GitHub:

* Clone the source: `git clone git@github.com:Macabresoft/Macabre2D.git`
* Pull down LFS objects: `git lfs fetch --all`
* Place your existing Macabre2D project or [pull down a template](https://github.com/Macabresoft/Macabre2D-Project-Template) into the `\Project\` folder
* Restore NuGet packages and build Macabre2D.sln

### Compiling Shaders on Linux

MonoGame has a guide for compiling Shaders on linux [here](https://docs.monogame.net/articles/getting_started/1_setting_up_your_development_environment_ubuntu.html#install-mgcb-editor).

### Using Fonts

When using custom fonts (non-system fonts) make sure to *install the font for all users* before using it within Macabre2D. This cannot be done from inside an archive folder (.zip, .rar, etc), so be sure to extract the font to your system,
right click, and select 'Install for All Users'. This is a limitation of the MonoGame Content Pipeline.

## Creating a Game

Your game's code must be directly integrated with Macabre2D, giving you full control of the engine and editor in the context of your game. As a result, the best way to create a game in Macabre2D is to create a fork of Macabre2D.

* Optionally fork Macabre2D
* Clone Macabre2D
* Create a separate repository for content which operates as a git repository inside of the `/Project/` folder ([use a template](https://github.com/Macabresoft/Macabre2D-Project-Template))
* Run the `Macabre2D.UI.Editor` project once to generate default content
* Add custom scripts to the Macabre2D.Scripts project (inside the Scripts folder, which is a link to inside `/Content`)

