namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a node in the content tree.
    /// </summary>
    public interface IContentNode {
        /// <summary>
        /// Occurs when the path to this file has changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> PathChanged;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        IContentDirectory Parent { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the name without an extension.
        /// </summary>
        string NameWithoutExtension { get; set; }

        /// <summary>
        /// Changes the parent of this node.
        /// </summary>
        /// <param name="newParent">The new parent.</param>
        void ChangeParent(IContentDirectory newParent);

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
        private string _name;

        /// <inheritdoc />
        public event EventHandler<ValueChangedEventArgs<string>> PathChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        protected ContentNode(string name, IContentDirectory parent) {
            this._name = name;
            this.ChangeParent(parent);
        }

        /// <inheritdoc />
        public abstract Guid Id { get; }

        /// <inheritdoc />
        public string Name {
            get => this._name;
            set {
                var originalPath = this.GetFullPath();
                if (string.IsNullOrEmpty(value)) {
                    this.RaisePropertyChanged(nameof(this.Name));
                }
                else if (this.Set(ref this._name, value, true)) {
                    if (this.Parent is IContentDirectory directory) {
                        directory.RemoveChild(this);
                        directory.AddChild(this);
                    }

                    this.OnPathChanged(originalPath);
                }
            }
        }

        /// <inheritdoc />
        [DataMember(Name = "Name")]
        [Category("File System")]
        public string NameWithoutExtension {
            get => this.GetNameWithoutExtension();
            set => this.Name = $"{value}{this.GetFileExtension()}";
        }

        /// <inheritdoc />
        public IContentDirectory Parent { get; private set; }

        /// <inheritdoc />
        public void ChangeParent(IContentDirectory newParent) {
            var originalParent = this.Parent;
            if (newParent != originalParent && newParent != this) {
                var originalPath = originalParent != null ? this.GetFullPath() : string.Empty;
                originalParent?.RemoveChild(this);

                if (newParent?.AddChild(this) == true) {
                    this.Parent = newParent;

                    if (originalParent != null) {
                        this.OnPathChanged(originalPath);
                    }
                }
                else {
                    originalParent?.AddChild(this);
                }
            }
        }

        /// <inheritdoc />
        public virtual string GetContentPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.NameWithoutExtension)) {
                return Path.Combine(this.Parent.GetContentPath(), this.NameWithoutExtension);
            }

            return this.NameWithoutExtension ?? string.Empty;
        }

        /// <inheritdoc />
        public int GetDepth() {
            return this.Parent?.GetDepth() + 1 ?? 0;
        }

        /// <inheritdoc />
        public virtual string GetFullPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this.Parent.GetFullPath(), this.Name);
            }

            return this.Name ?? string.Empty;
        }

        /// <inheritdoc />
        public bool IsDescendentOf(IContentDirectory directory) {
            var isDescendent = false;
            if (this.Parent != null) {
                isDescendent = this.Parent == directory || this.Parent.IsDescendentOf(directory);
            }

            return isDescendent;
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <returns>The file extension.</returns>
        protected abstract string GetFileExtension();

        /// <summary>
        /// Gets the name without its file extension.
        /// </summary>
        /// <returns>The name without its file extension.</returns>
        protected abstract string GetNameWithoutExtension();

        /// <summary>
        /// Called when the path changes.
        /// </summary>
        /// <param name="originalPath">The original path from before the change.</param>
        protected virtual void OnPathChanged(string originalPath) {
            this.RaisePathChanged(this, new ValueChangedEventArgs<string>(originalPath, this.GetFullPath()));
        }

        /// <summary>
        /// Raises the <see cref="PathChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        protected void RaisePathChanged(object sender, ValueChangedEventArgs<string> e) {
            this.PathChanged?.SafeInvoke(sender, e);
        }
    }
}