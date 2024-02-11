namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A wrapper for content being filtered for a custom tree view.
/// </summary>
public class FilteredContentWrapper : INameable {
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredContentWrapper" /> class.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="assetType">The asset type to find.</param>
    /// <param name="allowDirectorySelection">A value indicating whether or not directories are valid objects to select.</param>
    public FilteredContentWrapper(IContentNode node, Type assetType, bool allowDirectorySelection) : this(node, assetType, allowDirectorySelection, _ => true) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredContentWrapper" /> class.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="assetType">The asset type to find.</param>
    /// <param name="allowDirectorySelection">A value indicating whether or not directories are valid objects to select.</param>
    /// <param name="shouldDisplayFunc">A func describing whether or not a <see cref="IContentNode" /> should be displayed.</param>
    public FilteredContentWrapper(IContentNode node, Type assetType, bool allowDirectorySelection, Func<ContentFile, bool> shouldDisplayFunc) {
        if (assetType == null) {
            throw new ArgumentNullException(nameof(assetType));
        }

        this.Node = node ?? throw new ArgumentNullException(nameof(node));

        if (node is IContentDirectory directory) {
            var children = new List<FilteredContentWrapper>();
            foreach (var childDirectory in directory.Children.OfType<IContentDirectory>()) {
                if (allowDirectorySelection || childDirectory.GetAllContentFiles().Any(x => x.Asset is { IgnoreInBuild: false } && assetType.IsInstanceOfType(x.Asset) && shouldDisplayFunc(x))) {
                    children.Add(new FilteredContentWrapper(childDirectory, assetType, allowDirectorySelection, shouldDisplayFunc));
                }
            }

            children.AddRange(
                directory.Children
                    .OfType<ContentFile>()
                    .Where(x => x.Asset is { IgnoreInBuild: false } && assetType.IsInstanceOfType(x.Asset) && shouldDisplayFunc(x))
                    .Select(x => new FilteredContentWrapper(x, assetType, allowDirectorySelection, shouldDisplayFunc)));

            this.Children = children;
        }
        else {
            this.Children = Enumerable.Empty<FilteredContentWrapper>();
        }
    }

    /// <summary>
    /// Gets the children of this node if any exist.
    /// </summary>
    public IEnumerable<FilteredContentWrapper> Children { get; }

    /// <summary>
    /// Gets the node this is wrapping.
    /// </summary>
    public IContentNode Node { get; }

    /// <inheritdoc />
    public string Name {
        get => this.Node.Name;
        set => this.Node.Name = value;
    }

    /// <summary>
    /// Gets a value indicating whether or not this is a directory.
    /// </summary>
    private bool IsDirectory => this.Node is IContentDirectory;

    /// <summary>
    /// Gets all files under and including this node.
    /// </summary>
    /// <returns>All files.</returns>
    public IEnumerable<FilteredContentWrapper> GetAllFiles() {
        var files = new List<FilteredContentWrapper>();
        this.AddAllFiles(files);
        return files;
    }

    private void AddAllFiles(ICollection<FilteredContentWrapper> files) {
        if (!this.IsDirectory) {
            files.Add(this);
        }
        else {
            foreach (var child in this.Children) {
                child.AddAllFiles(files);
            }
        }
    }
}