namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A node in the content tree.
    /// </summary>
    public abstract class ContentNode : NotifyPropertyChanged {
        private ContentDirectory _parent;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ContentNode(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }
            
            this._name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
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

        /// <summary>
        /// Gets the content path to this file, which assumes a root of the project content directory.
        /// </summary>
        /// <returns>The content path.</returns>
        public string GetContentPath() {
            if (this._parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this._parent.GetContentPath(), this.Name);
            }

            return this.Name ?? string.Empty;
        }

        /// <summary>
        /// Gets the full path to this piece of content.
        /// </summary>
        /// <returns>The file path.</returns>
        public string GetPath() {
            if (this._parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this._parent.GetPath(), this.Name);
            }

            return this.Name ?? string.Empty;
        }

        /// <summary>
        /// Initializes this node with a parent.
        /// </summary>
        /// <param name="parent"></param>
        public void Initialize(ContentDirectory parent) {
            this._parent = parent;
        }
    }
}