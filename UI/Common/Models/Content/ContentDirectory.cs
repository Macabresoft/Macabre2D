namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;

/// <summary>
/// Interface for a directory content node.
/// </summary>
public interface IContentDirectory : IContentNode {
    /// <summary>
    /// Gets the children of this directory.
    /// </summary>
    IReadOnlyCollection<IContentNode> Children { get; }

    /// <summary>
    /// Adds the child node.
    /// </summary>
    /// <param name="node">The child node to add.</param>
    bool AddChild(IContentNode node);

    /// <summary>
    /// Gets a value indicating whether or not the specified metadata is a descendent of this directory.
    /// </summary>
    /// <param name="contentId">The metadata identifier.</param>
    /// <returns>A value indicating whether or not the specified metadata is a descendent of this directory.</returns>
    bool ContainsMetadata(Guid contentId);

    /// <summary>
    /// Gets all content files that are descendants of this directory.
    /// </summary>
    /// <returns>All content files that are descendants of this directory.</returns>
    IEnumerable<ContentFile> GetAllContentFiles();

    /// <summary>
    /// Loads the child directories under this node.
    /// </summary>
    /// <param name="fileSystemService">The file service.</param>
    void LoadChildDirectories(IFileSystemService fileSystemService);

    /// <summary>
    /// Removes the child node.
    /// </summary>
    /// <param name="node">The child node to remove.</param>
    bool RemoveChild(IContentNode node);

    /// <summary>
    /// Finds a content node if it exists.
    /// </summary>
    /// <param name="splitContentPath"></param>
    /// <returns>The content node or null.</returns>
    IContentNode TryFindNode(string[] splitContentPath);

    /// <summary>
    /// Tries to find a content node by its identifier.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <param name="file">The file.</param>
    /// <returns>A value indicating whether or not the file was found.</returns>
    bool TryFindNode(Guid contentId, out ContentFile file);

    /// <summary>
    /// Tries to find a content node.
    /// </summary>
    /// <param name="splitContentPath">The split content path.</param>
    /// <param name="node">The found content node.</param>
    /// <returns>A value indicating whether or not the node was found.</returns>
    bool TryFindNode(string[] splitContentPath, out IContentNode node);
}

/// <summary>
/// A directory content node.
/// </summary>
[DataContract(Name = "Directory")]
public class ContentDirectory : ContentNode, IContentDirectory {
    private readonly ObservableCollectionExtended<IContentNode> _children = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentNode" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="parent">The parent content directory.</param>
    public ContentDirectory(string name, IContentDirectory parent) {
        this.Initialize(name, parent);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IContentNode> Children => this._children;

    /// <inheritdoc />
    public override Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public bool AddChild(IContentNode node) {
        var result = false;
        if (node != null && !this._children.Contains(node)) {
            var directory = node as IContentDirectory;
            var isDirectory = directory != null;
            if (!(isDirectory && this.IsDescendentOf(directory))) {
                for (var i = 0; i < this._children.Count; i++) {
                    var child = this._children[i];
                    if (isDirectory) {
                        if (child is IContentDirectory) {
                            if (StringComparer.OrdinalIgnoreCase.Compare(node.Name, child.Name) < 0) {
                                this._children.Insert(i, node);
                                result = true;
                                break;
                            }
                        }
                        else {
                            this._children.Insert(i, node);
                            result = true;
                            break;
                        }
                    }
                    else if (child is not IContentDirectory) {
                        if (StringComparer.OrdinalIgnoreCase.Compare(node.Name, child.Name) < 0) {
                            this._children.Insert(i, node);
                            result = true;
                            break;
                        }
                    }
                }

                if (!result) {
                    this._children.Add(node);
                    result = true;
                }

                node.PathChanged += this.Child_PathChanged;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public bool ContainsMetadata(Guid contentId) {
        if (contentId != Guid.Empty) {
            for (var i = this._children.Count - 1; i >= 0; i--) {
                var child = this._children[i];
                switch (child) {
                    case ContentFile file when file.Metadata?.ContentId == contentId:
                    case IContentDirectory directory when directory.ContainsMetadata(contentId):
                        return true;
                }
            }
        }

        return false;
    }

    /// <inheritdoc />
    public IEnumerable<ContentFile> GetAllContentFiles() {
        var contentFiles = this.Children.OfType<ContentFile>().ToList();
        foreach (var child in this.Children.OfType<IContentDirectory>()) {
            contentFiles.AddRange(child.GetAllContentFiles());
        }

        return contentFiles;
    }

    /// <inheritdoc />
    public virtual void LoadChildDirectories(IFileSystemService fileSystemService) {
        var currentDirectoryPath = this.GetFullPath();

        if (fileSystemService.DoesDirectoryExist(currentDirectoryPath)) {
            var directories = fileSystemService.GetDirectories(currentDirectoryPath);

            foreach (var directory in directories) {
                this.LoadDirectory(fileSystemService, directory);
            }
        }
    }

    /// <inheritdoc />
    public bool RemoveChild(IContentNode node) {
        node.PathChanged -= this.Child_PathChanged;
        return this._children.Remove(node);
    }


    /// <inheritdoc />
    public IContentNode TryFindNode(string[] splitContentPath) {
        IContentNode node = null;
        var parentDepth = splitContentPath.Length - 1;
        var currentDepth = this.GetDepth();

        if (currentDepth == parentDepth) {
            var nodeName = splitContentPath[currentDepth];
            node = this.Children.FirstOrDefault(x => x.NameWithoutExtension == nodeName);
        }
        else if (currentDepth < parentDepth) {
            var parentName = splitContentPath[currentDepth];
            if (this._children.FirstOrDefault(x => x.Name == parentName) is IContentDirectory child) {
                node = child.TryFindNode(splitContentPath);
            }
        }

        return node;
    }

    /// <inheritdoc />
    public bool TryFindNode(Guid contentId, out ContentFile file) {
        file = this.Children.FirstOrDefault(x => x.Id == contentId) as ContentFile;
        if (file == null) {
            foreach (var child in this.Children.OfType<IContentDirectory>()) {
                if (child.TryFindNode(contentId, out file)) {
                    break;
                }
            }
        }

        return file != null;
    }

    /// <inheritdoc />
    public bool TryFindNode(string[] splitContentPath, out IContentNode node) {
        node = this.TryFindNode(splitContentPath);
        return node != null;
    }

    /// <inheritdoc />
    protected override string GetFileExtension() {
        return string.Empty;
    }

    /// <inheritdoc />
    protected override string GetNameWithoutExtension() {
        return this.Name;
    }

    /// <summary>
    /// Loads a directory.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path to the directory.</param>
    protected void LoadDirectory(IFileSystemService fileSystem, string path) {
        var splitPath = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var name = splitPath.Length > 1 ? splitPath.Last() : path;
        var node = new ContentDirectory(name, this);
        node.LoadChildDirectories(fileSystem);
    }

    private void Child_PathChanged(object sender, ValueChangedEventArgs<string> e) {
        this.RaisePathChanged(sender, e);
    }
}