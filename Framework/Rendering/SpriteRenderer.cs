namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>
/// An entity which will render a single sprite.
/// </summary>
[Category(CommonCategories.Rendering)]
public class SpriteRenderer : BaseSpriteEntity {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteRenderer" /> class.
    /// </summary>
    public SpriteRenderer() : base() {
    }

    /// <inheritdoc />
    public override byte? SpriteIndex => this.SpriteReference.SpriteIndex;

    /// <summary>
    /// Gets the sprite reference this entity will render.
    /// </summary>
    [DataMember(Order = 0, Name = "Sprite")]
    public SpriteReference SpriteReference { get; } = new();

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this.SpriteReference.Asset;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.SpriteReference.PropertyChanged -= this.SpriteReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SpriteReference;
    }

    private void SpriteReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(this.SpriteSheet.SpriteSize)) {
            this.Reset();
        }
    }
}