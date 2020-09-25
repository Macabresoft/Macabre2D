# Macabre2D (Macabresoft.MonoGame)

Common MonoGame functionality used across Macabresoft products. A framework built on top of another framework.

## Supported Platforms

Macabresoft.MonoGame.Core and its sample projects have been confirmed to run on modern Windows, MacOS, and Linux (Ubuntu) as of 2020.

## Git LFS

You must install Git LFS (Large File Storage) to use this repository. [This article](https://help.github.com/en/articles/installing-git-large-file-storage) should assist you.

## Source Code

The full source code is available here from GitHub:

* Clone the source: `git clone https://github.com/BrettStory/Macabresoft.MonoGame`
* Pull down LFS objects: `git lfs pull` and `git lfs fetch --all`
* Restore NuGet packages for Macabresoft.MonoGame.Core.sln.

## Using Fonts
 
When using custom fonts (non-system fonts) make sure to *install the font for all users* before using it within Macabre2D. This cannot be done from inside an archive folder (.zip, .rar, etc), so be sure to extract the font to your system, right click, and select 'Install for All Users'.  This is a limitation of the MonoGame Content Pipeline.