namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A <see cref="ContentFile" /> for <see cref="SpriteSheet" />.
/// </summary>
public class SpriteSheetContentFile : ContentFile<SpriteSheet> {
    private readonly List<INameableCollection> _children = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentNode" /> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="metadata">The metadata.</param>
    public SpriteSheetContentFile(IContentDirectory parent, ContentMetadata metadata) : base(parent, metadata) {
        this._children.Add(this.TypedAsset.AutoTileSets);
        this._children.Add(this.TypedAsset.SpriteAnimations);
        this._children.Add(this.TypedAsset.Fonts);
        this._children.Add(this.TypedAsset.GamePadIconSets);
        this._children.Add(this.TypedAsset.KeyboardIconSets);
    }

    /// <summary>
    /// Gets the children.
    /// </summary>
    public IReadOnlyCollection<INameableCollection> Children => this._children;
}