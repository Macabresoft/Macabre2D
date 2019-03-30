# Macabre2D

A 2D game engine built on top of the MonoGame framework. This project was started on April 23, 2016.

## Supported Platforms

Currently, the Macabre2D editor only supports Windows. The framework itself is technically setup to work with any platform; however, only Windows has been tested.

## Source Code

The full source code is available here from GitHub:

 * Clone the source: `git clone https://github.com/BrettStory/Macabre2D`
 * Set up the root submodules by calling the following at the base of the repository: `git submodule update --init`
 * Set up the MonoGame submodules by calling the following in the MonoGame folder: `git submodule update --init`
 * Run MonoGame/Protobuild.exe to generate MonoGame project files and solutions.
 * Run BuildMonoGame.bat to build the MonoGame solutions required by Macabre2D.
 * Install MonoGame for your version of Visual Studio: http://www.monogame.net/downloads/
 * Restore NuGet packages for Macabre2D.sln.
 * Open Macabre2D.sln. From here you can run the editor or example projects.