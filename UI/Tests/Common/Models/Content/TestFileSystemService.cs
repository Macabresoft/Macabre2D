namespace Macabresoft.Macabre2D.UI.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Macabresoft.Macabre2D.UI.Common;

internal class TestFileSystemService : IFileSystemService {
    public TestFileSystemService() {
        this.FillDirectoryStructure();
    }

    public IDictionary<string, IEnumerable<string>> DirectoryToChildrenMap { get; } = new Dictionary<string, IEnumerable<string>>();

    public IPathService TestPathService { get; } = new PathService(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

    public void CopyFile(string originalPath, string newPath) {
    }

    public void CreateDirectory(string path) {
    }

    public void DeleteDirectory(string path) {
    }

    public void DeleteFile(string path) {
    }

    public bool DoesDirectoryExist(string path) {
        var result = false;
        var splitPath = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        if (splitPath.Length == 1) {
            result = this.DirectoryToChildrenMap.ContainsKey(path);
        }
        else {
            var hasPath = true;
            for (var i = 1; i < splitPath.Length; i++) {
                var precedingDirectory = splitPath[i - 1];
                var currentDirectory = splitPath[i];
                if (!this.DirectoryToChildrenMap.TryGetValue(precedingDirectory, out var children) && children != null && !children.Contains(currentDirectory)) {
                    hasPath = false;
                    break;
                }
            }

            result = hasPath;
        }

        return result;
    }

    public bool DoesFileExist(string path) {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetDirectories(string directoryPath) {
        var result = new List<string>();
        if (this.DoesDirectoryExist(directoryPath)) {
            var split = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (this.DirectoryToChildrenMap.TryGetValue(split.Last(), out var children)) {
                result.AddRange(children);
            }
        }

        return result;
    }

    public IEnumerable<string> GetDirectories(string directoryPath, string searchPattern) {
        return Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetFiles(string directoryPath) {
        return Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetFiles(string directoryPath, string searchPattern) {
        return Enumerable.Empty<string>();
    }

    public bool IsValidFileOrDirectoryName(string name) {
        return true;
    }

    public void MoveDirectory(string originalPath, string newPath) {
    }

    public void MoveFile(string originalPath, string newPath) {
    }

    public void OpenDirectoryInFileExplorer(string directoryPath) {
    }

    public void WriteAllText(string filePath, string text) {
    }

    private void FillDirectoryStructure() {
        this.DirectoryToChildrenMap.Clear();
        var directories = new List<string>();
        this.DirectoryToChildrenMap[this.TestPathService.PlatformsDirectoryPath] = new[] { PathService.ContentDirectoryName };
        this.DirectoryToChildrenMap[PathService.ContentDirectoryName] = directories;
        for (var i = 0; i < 3; i++) {
            var name = Guid.NewGuid();
            this.GenerateFakeDirectory(name, 3);
            directories.Add(name.ToString());
        }
    }

    private void GenerateFakeDirectory(Guid parentDirectoryName, int numberOfChildren) {
        var directoryNames = new List<string>();
        for (var i = 0; i < numberOfChildren; i++) {
            var id = Guid.NewGuid();
            directoryNames.Add(id.ToString());

            if (i > 0 && i % 2 == 0) {
                this.GenerateFakeDirectory(id, 2);
            }
            else {
                this.DirectoryToChildrenMap[id.ToString()] = Enumerable.Empty<string>();
            }
        }

        this.DirectoryToChildrenMap[parentDirectoryName.ToString()] = directoryNames;
    }
}