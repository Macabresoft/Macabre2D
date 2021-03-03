namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Services;

    /// <summary>
    /// Interface for a directory content node.
    /// </summary>
    public interface IContentDirectory : IContentNode {
        /// <summary>
        /// Gets the children of this directory.
        /// </summary>
        IReadOnlyCollection<ContentNode> Children { get; }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="node">The child node to add.</param>
        void AddChild(ContentNode node);

        /// <summary>
        /// Finds a content node if it exists.
        /// </summary>
        /// <param name="splitContentPath"></param>
        /// <returns>The content node or null.</returns>
        IContentNode FindNode(string[] splitContentPath);

        /// <summary>
        /// Loads the child directories under this node.
        /// </summary>
        /// <param name="fileSystemService">The file service.</param>
        void LoadChildDirectories(IFileSystemService fileSystemService);

        /// <summary>
        /// Removes the child node.
        /// </summary>
        /// <param name="node">The child node to remove.</param>
        void RemoveChild(ContentNode node);

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
    public class ContentDirectory : ContentNode, IContentDirectory {
        private readonly ObservableCollectionExtended<ContentNode> _children = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent content directory.</param>
        public ContentDirectory(string name, IContentDirectory parent) : base(name, parent) {
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ContentNode> Children => this._children;

        /// <inheritdoc />
        public override string NameWithoutExtension => this.Name;

        /// <inheritdoc />
        public void AddChild(ContentNode node) {
            this._children.Add(node);
        }

        /// <inheritdoc />
        public IContentNode FindNode(string[] splitContentPath) {
            // TODO: allow content files to figure out their current depth and optimize this based on that. If the current depth matches the length of splitContentLength, you know you should be checking all children. Otherwise...
            IContentNode node = null;
            if (splitContentPath.Length == 1) {
                node = this.Children.FirstOrDefault(x => x.GetContentPath() == Path.Combine(splitContentPath));
            }
            if (splitContentPath.Any()) {
                foreach (var child in this._children) {
                    if (child.GetContentPath() == Path.Combine(splitContentPath)) {
                        node = child;
                        break;
                    }
                    else if (child is IContentDirectory directory && directory.TryFindNode(splitContentPath, out node)) {
                        break;
                    }
                }
            }

            return node;
        }

        /// <inheritdoc />
        public void LoadChildDirectories(IFileSystemService fileSystemService) {
            var currentDirectoryPath = this.GetFullPath();

            if (fileSystemService.DoesDirectoryExist(currentDirectoryPath)) {
                var directories = fileSystemService.GetDirectories(currentDirectoryPath).Where(x => Path.GetDirectoryName(x)?.StartsWith('.') == false);

                foreach (var directory in directories) {
                    this.LoadDirectory(fileSystemService, directory);
                }
            }
        }

        /// <inheritdoc />
        public void RemoveChild(ContentNode node) {
            this._children.Remove(node);
        }

        /// <inheritdoc />
        public bool TryFindNode(string[] splitContentPath, out IContentNode node) {
            node = this.FindNode(splitContentPath);
            return node != null;
        }

        private void LoadDirectory(IFileSystemService fileSystemService, string path) {
            var splitPath = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var name = splitPath.Length > 1 ? splitPath.Last() : path;
            var node = new ContentDirectory(name, this);
            node.LoadChildDirectories(fileSystemService);
        }
    }
}