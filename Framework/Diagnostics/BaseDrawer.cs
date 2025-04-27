namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for drawing the outlines of entities.
/// </summary>
public abstract class BaseDrawer : RenderableEntity {
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 0)]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the line thickness.
    /// </summary>
    /// <value>The line thickness.</value>
    [DataMember(Order = 1)]
    public float LineThickness { get; set; } = 1f;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public override RenderPriority RenderPriority { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this should use dynamic line thickness.
    /// </summary>
    /// <value><c>true</c> if this should use dynamic line thickness; otherwise, <c>false</c>.</value>
    [DataMember(Order = 2)]
    public bool UseDynamicLineThickness { get; set; }

    /// <summary>
    /// Gets the primitive drawer.
    /// </summary>
    /// <value>The primitive drawer.</value>
    protected PrimitiveDrawer? PrimitiveDrawer { get; private set; }

    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this.SpriteBatch != null) {
            this.PrimitiveDrawer = PrimitiveDrawer.GetInstance(this.SpriteBatch);
        }
    }

    /// <summary>
    /// Gets the line thickness.
    /// </summary>
    /// <param name="viewHeight">Height of the view.</param>
    /// <returns>The appropriate line thickness for this drawer.</returns>
    protected float GetLineThickness(float viewHeight) {
        var result = this.LineThickness;
        if (this.UseDynamicLineThickness && this.Game.GraphicsDevice is { } device) {
            result = LineHelper.GetDynamicLineThickness(result, viewHeight, device, this.Project);
        }

        return result;
    }
}