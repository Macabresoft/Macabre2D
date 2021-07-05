namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Common.Services;

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
        /// Finds a content node if it exists.
        /// </summary>
        /// <param name="splitContentPath"></param>
        /// <returns>The content node or null.</returns>
        IContentNode FindNode(string[] splitContentPath);

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
        /// Tries to find a content node.
        /// </summary>
        /// <param name="splitContentPath">The split content path.</param>
        /// <param name="node">The found content node.</param>
        /// <returns>A value indicating whether or not the node was found.</returns>
        bool TryFindNode(string[] splitContentPath, out IContentNode node);

        /// <summary>
        /// Gets a value indicating whether or not the specified metadata is a descendent of this directory.
        /// </summary>
        /// <param name="contentId">The metadata identifier.</param>
        /// <returns>A value indicating whether or not the specified metadata is a descendent of this directory.</returns>
        bool ContainsMetadata(Guid contentId);
    }

    /// <summary>
    /// A directory content node.
    /// </summary>
    public class ContentDirectory : ContentNode, IContentDirectory {
        private readonly ObservableCollectionExtended<IContentNode> _children = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent content directory.</param>
        public ContentDirectory(string name, IContentDirectory parent) : base(name, parent) {
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IContentNode> Children => this._children;

        /// <inheritdoc />
        public override string NameWithoutExtension => this.Name;

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
        public IContentNode FindNode(string[] splitContentPath) {
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
                    node = child.FindNode(splitContentPath);
                }
            }

            return node;
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
            return this._children.Remove(node);
        }

        /// <inheritdoc />
        public bool TryFindNode(string[] splitContentPath, out IContentNode node) {
            node = this.FindNode(splitContentPath);
            return node != null;
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
}