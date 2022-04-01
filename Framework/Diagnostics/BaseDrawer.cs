namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A base class for drawing the outlines of entities.
/// </summary>
public abstract class BaseDrawer : RenderableEntity {
    private Color _color = Color.White;
    private float _lineThickness = 1f;
    private bool _useDynamicLineThickness;

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 0)]
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets or sets the line thickness.
    /// </summary>
    /// <value>The line thickness.</value>
    [DataMember(Order = 1)]
    public float LineThickness {
        get => this._lineThickness;
        set => this.Set(ref this._lineThickness, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this should use dynamic line thickness.
    /// </summary>
    /// <value><c>true</c> if this should use dynamic line thickness; otherwise, <c>false</c>.</value>
    [DataMember(Order = 2)]
    public bool UseDynamicLineThickness {
        get => this._useDynamicLineThickness;
        set => this.Set(ref this._useDynamicLineThickness, value);
    }

    /// <summary>
    /// Gets the primitive drawer.
    /// </summary>
    /// <value>The primitive drawer.</value>
    protected PrimitiveDrawer? PrimitiveDrawer { get; private set; }

    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this.SpriteBatch != null) {
            this.PrimitiveDrawer = this.Scene.ResolveDependency(
                () => new PrimitiveDrawer(this.SpriteBatch));
        }
    }

    /// <summary>
    /// Gets the line thickness.
    /// </summary>
    /// <param name="viewHeight">Height of the view.</param>
    /// <returns>The appropriate line thickness for this drawer.</returns>
    protected float GetLineThickness(float viewHeight) {
        var result = this.LineThickness;
        if (this.UseDynamicLineThickness && this.Game.GraphicsDevice is GraphicsDevice device) {
            result = LineHelper.GetDynamicLineThickness(result, viewHeight, device, this.Settings);
        }

        return result;
    }
}