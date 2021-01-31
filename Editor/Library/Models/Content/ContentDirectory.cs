namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.Collections.Generic;
    using Macabresoft.Core;

    public class ContentDirectory : ContentNode {
        private readonly ObservableCollectionExtended<ContentFile> _contentFiles = new();

        public ContentDirectory(string name) : base(name) {
        }

        public IReadOnlyCollection<ContentFile> ContentFiles => this._contentFiles;
    }
}