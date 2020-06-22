# Macabre2D

A 2D game framework built on top of the MonoGame framework. This project was started on April 23, 2016 (some early repository history was lost).

## Supported Platforms

Macabre2D.Framework and its example projects have been confirmed to run on modern Windows, MacOS, and Linux (Ubuntu) as of 2020.

## Git LFS

You must install Git LFS (Large File Storage) to use this repository. [This article](https://help.github.com/en/articles/installing-git-large-file-storage) should assist you.

## Source Code

The full source code is available here from GitHub:

* Clone the source: `git clone https://github.com/BrettStory/Macabre2D.Framework`
* Pull down LFS objects: `git lfs pull` and `git lfs fetch --all`
* Install MonoGame for your version of Visual Studio: http://www.monogame.net/downloads/
    * Macabre2D is built with MonoGame 3.7 in mind.
* Restore NuGet packages for Macabre2D.Framework.sln.

## Using Fonts
 
When using custom fonts (non-system fonts) make sure to *install the font for all users* before using it within Macabre2D. This cannot be done from inside an archive folder (.zip, .rar, etc), so be sure to extract the font to your system, right click, and select 'Install for All Users'.  This is a limitation of the MonoGame Content Pipeline.