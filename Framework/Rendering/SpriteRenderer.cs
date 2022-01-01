namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity which will render a single sprite.
/// </summary>
[Display(Name = "Sprite Renderer")]
[Category(CommonCategories.Rendering)]
public sealed class SpriteRenderer : BaseSpriteEntity {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteRenderer" /> class.
    /// </summary>
    public SpriteRenderer() : base() {
        this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override byte? SpriteIndex => this.SpriteReference.SpriteIndex;

    /// <summary>
    /// Gets the sprite reference this entity will render.
    /// </summary>
    [DataMember(Order = 0, Name = "Sprite")]
    public SpriteReference SpriteReference { get; } = new();

    /// <inheritdoc />
    public override SpriteSheet? SpriteSheet => this.SpriteReference.Asset;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        
        this.SpriteReference.PropertyChanged -= this.SpriteReference_PropertyChanged;
        this.Scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this.SpriteReference);
        this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
    }

    private void SpriteReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.SpriteSheet.SpriteSize)) {
            this.Reset();
        }
        else if (e.PropertyName == nameof(this.SpriteSheet.ContentId)) {
            this.Scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this.SpriteReference);
            this.Reset();
        }
    }
}