﻿namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.IO;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a node in the content tree.
    /// </summary>
    public interface IContentNode {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name without an extension.
        /// </summary>
        string NameWithoutExtension { get; }

        /// <summary>
        /// Gets the content path to this file or directory, which assumes a root of the project content directory.
        /// </summary>
        /// <returns>The content path.</returns>
        string GetContentPath();

        /// <summary>
        /// Gets the depth of this node in the content hierarchy. The root is 0 and each directory deeper is 1.
        /// </summary>
        /// <returns>The depth.</returns>
        int GetDepth();

        /// <summary>
        /// Gets the full path to this file or directory.
        /// </summary>
        /// <returns>The full path.</returns>
        string GetFullPath();

        /// <summary>
        /// Checks whether or not this node is a descendent of the provided directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>A value indicating whether or not this node is a descendent of the provided directory.</returns>
        bool IsDescendentOf(IContentDirectory directory);
    }

    /// <summary>
    /// A node in the content tree.
    /// </summary>
    public abstract class ContentNode : NotifyPropertyChanged, IContentNode {
        private readonly IContentDirectory _parent;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        protected ContentNode(string name, IContentDirectory parent) {
            this._name = name;
            this._parent = parent;
            this._parent?.AddChild(this);
        }

        /// <inheritdoc />
        public virtual string NameWithoutExtension => Path.GetFileNameWithoutExtension(this.Name);

        /// <inheritdoc />
        public string Name {
            get => this._name;
            set {
                if (string.IsNullOrEmpty(value)) {
                    this.RaisePropertyChanged(nameof(this.Name));
                }
                else {
                    this.Set(ref this._name, value, true);
                }
            }
        }

        /// <inheritdoc />
        public virtual string GetContentPath() {
            if (this._parent != null && !string.IsNullOrEmpty(this.NameWithoutExtension)) {
                return Path.Combine(this._parent.GetContentPath(), this.NameWithoutExtension);
            }

            return this.NameWithoutExtension ?? string.Empty;
        }

        /// <inheritdoc />
        public int GetDepth() {
            return this._parent?.GetDepth() + 1 ?? 0;
        }

        /// <inheritdoc />
        public virtual string GetFullPath() {
            if (this._parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this._parent.GetFullPath(), this.Name);
            }

            return this.Name ?? string.Empty;
        }

        /// <inheritdoc />
        public bool IsDescendentOf(IContentDirectory directory) {
            var isDescendent = false;
            if (this._parent != null) {
                isDescendent = this._parent == directory || this._parent.IsDescendentOf(directory);
            }

            return isDescendent;
        }
    }
}