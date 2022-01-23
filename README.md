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
* Restore NuGet packages for Macabre2D.sln.

### Using Fonts
 
When using custom fonts (non-system fonts) make sure to *install the font for all users* before using it within Macabre2D. This cannot be done from inside an archive folder (.zip, .rar, etc), so be sure to extract the font to your system, right click, and select 'Install for All Users'. This is a limitation of the MonoGame Content Pipeline.

## Creating a Game

Your game's code must be directly integrated with Macabre2D, giving you full control of the engine and editor in the context of your game. As a result, the best way to create a game in Macabre2D is to create a fork of Macabre2D.

* Fork Macabre2D
* Optionally create a branch for your game
* Remove the following lines from your `.gitignore` file:
  * `/Content/`
  * `/Gameplay/`
* Run the `Macabre2D.UI.Editor` project once to generate default content

At this point, you can fill your `/Content/` folder with images, audio, fonts, and anything else your game engine will need to read from storage

The `Macabre2D.Gameplay` project should be used as a container for all custom Entities and Systems your game may need. This keeps the core logic separate from your game's logic. This will make merging easier in the event that you fix a bug or add a new feature that you would like to contribute to the main Macabre2D project.

I highly recommend all changes to `.gitignore`, `/Gameplay/`, and `/Content/` be kept separated into their own commits, as it makes merging contributions to Macabre2D easier. This is more of a recommendation than a requirement, though.