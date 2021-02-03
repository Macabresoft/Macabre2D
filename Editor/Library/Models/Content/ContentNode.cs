namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.IO;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a node in the content tree.
    /// </summary>
    public interface IContentNode {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the content path to this file or directory, which assumes a root of the project content directory.
        /// </summary>
        /// <returns>The content path.</returns>
        string GetContentPath();
    }

    /// <summary>
    /// A node in the content tree.
    /// </summary>
    public abstract class ContentNode : NotifyPropertyChanged, IContentNode {
        private string _name;
        private IContentDirectory? _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ContentNode(string name) : this(name, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ContentNode(string name, IContentDirectory? parent) {
            this._name = name;
        }

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
        public string GetContentPath() {
            if (this._parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this._parent.GetContentPath(), this.Name);
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