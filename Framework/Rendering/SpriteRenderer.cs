namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// Interface for an entity which renders a single sprite.
/// </summary>
public interface ISpriteRenderer : ISpriteEntity {
    /// <summary>
    /// Gets the sprite reference this entity will render.
    /// </summary>
    SpriteReference SpriteReference { get; }
}

/// <summary>
/// An entity which will render a single sprite.
/// </summary>
[Category(CommonCategories.Rendering)]
public class SpriteRenderer : BaseSpriteEntity, ISpriteRenderer {
    /// <inheritdoc />
    public override byte? SpriteIndex => this.SpriteReference.SpriteIndex;

    /// <inheritdoc />
    [DataMember(Order = 0, Name = "Sprite")]
    public SpriteReference SpriteReference { get; } = new();

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this.SpriteReference.Asset;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.SpriteReference.AssetChanged -= this.SpriteReference_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.SpriteReference.AssetChanged += this.SpriteReference_AssetChanged;
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SpriteReference;
    }

    private void SpriteReference_AssetChanged(object? sender, bool e) {
        this.Reset();
    }
}