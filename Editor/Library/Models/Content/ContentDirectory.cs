namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.Collections.Generic;
    using Macabresoft.Core;

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
        /// Removes the child node.
        /// </summary>
        /// <param name="node">The child node to remove.</param>
        void RemoveChild(ContentNode node);
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
        public ContentDirectory(string name) : base(name) {
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ContentNode> Children => this._children;

        /// <inheritdoc />
        public void AddChild(ContentNode node) {
            this._children.Add(node);
            node.Initialize(this);
        }

        /// <inheritdoc />
        public void RemoveChild(ContentNode node) {
            this._children.Remove(node);
        }
    }
}