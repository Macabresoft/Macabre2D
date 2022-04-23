namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A breakable block that displays a different sprite depending on if it is broken.
/// </summary>
public class BreakableSpriteBlock : BreakableBlock {
    private SpriteRenderer? _spriteRenderer;

    /// <summary>
    /// Gets the sprite reference that displays when this is broken.
    /// </summary>
    [DataMember]
    [Category("Breakable")]
    public SpriteReference BrokenSprite { get; } = new();

    /// <summary>
    /// Gets the sprite reference that displays when this is not broken.
    /// </summary>
    [DataMember]
    [Category("Breakable")]
    public SpriteReference Sprite { get; } = new();

    /// <summary>
    /// Gets or sets the render layers when this is broken.
    /// </summary>
    [DataMember]
    [Category("Breakable")]
    public Layers BrokenRenderLayers { get; set; } = Layers.Default;

    /// <summary>
    /// Gets or sets the render layers when this is not broken.
    /// </summary>
    [DataMember]
    [Category("Breakable")]
    public Layers RenderLayers { get; set; } = Layers.Default;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.Sprite.Initialize(this.Scene.Assets);
        this.BrokenSprite.Initialize(this.Scene.Assets);

        this._spriteRenderer = this.GetOrAddChild<SpriteRenderer>();
        this._spriteRenderer.Layers = this.RenderLayers;

        if (this.Sprite.Asset != null) {
            this._spriteRenderer.SpriteReference.LoadAsset(this.Sprite.Asset);
            this._spriteRenderer.SpriteReference.SpriteIndex = this.Sprite.SpriteIndex;
        }
    }

    /// <inheritdoc />
    protected override void OnBroken() {
        if (this._spriteRenderer != null) {
            this._spriteRenderer.Layers = this.BrokenRenderLayers;

            if (this.BrokenSprite.Asset != null) {
                this._spriteRenderer.SpriteReference.LoadAsset(this.BrokenSprite.Asset);
                this._spriteRenderer.SpriteReference.SpriteIndex = this.BrokenSprite.SpriteIndex;
            }
            else {
                this._spriteRenderer.SpriteReference.Clear();
            }
        }
    }
}