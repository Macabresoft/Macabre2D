namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Core;

    /// <summary>
    /// A wrapper for content being filtered for a custom tree view.
    /// </summary>
    public class FilteredContentWrapper {
        private readonly ObservableCollectionExtended<FilteredContentWrapper> _children = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredContentWrapper" /> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="assetType">The asset type to find.</param>
        public FilteredContentWrapper(IContentNode node, Type assetType) {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            this.Node = node ?? throw new ArgumentNullException(nameof(node));

            if (node is IContentDirectory directory) {
                var children = new List<FilteredContentWrapper>();
                foreach (var childDirectory in directory.Children.OfType<IContentDirectory>()) {
                    if (childDirectory.GetAllContentFiles().Any(x => x.Asset != null && assetType.IsInstanceOfType(x.Asset))) {
                        children.Add(new FilteredContentWrapper(childDirectory, assetType));
                    }
                }

                children.AddRange(
                    directory.Children
                        .OfType<ContentFile>()
                        .Where(x => x.Asset != null && assetType.IsInstanceOfType(x.Asset))
                        .Select(x => new FilteredContentWrapper(x, assetType)));

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

        /// <summary>
        /// Gets a value indicating whether or not this is a directory.
        /// </summary>
        private bool IsDirectory => this.Node is IContentDirectory;
    }
}