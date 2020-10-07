# Macabre2D (Macabresoft.MonoGame)

Common MonoGame functionality used across Macabresoft products. A framework built on top of another framework. Will eventually be a full-fledged editor/engine.

## Supported Platforms

Macabresoft.MonoGame.Core2D and its sample projects have been confirmed to run on modern Windows, MacOS, and Linux (Ubuntu) as of 2020. The only exception is the Avalonia project, which still has a dependency on DirectX.

## Git LFS

You must install Git LFS (Large File Storage) to use this repository. [This article](https://help.github.com/en/articles/installing-git-large-file-storage) should assist you.

## Source Code

The full source code is available here from GitHub:

* Clone the source: `git clone git@github.com:Macabresoft/Macabre2D.git`
* Pull down LFS objects: `git lfs pull` and `git lfs fetch --all`
* Restore NuGet packages for Macabre2D.sln.

## Using Fonts
 
When using custom fonts (non-system fonts) make sure to *install the font for all users* before using it within Macabre2D. This cannot be done from inside an archive folder (.zip, .rar, etc), so be sure to extract the font to your system, right click, and select 'Install for All Users'. This is a limitation of the MonoGame Content Pipeline.