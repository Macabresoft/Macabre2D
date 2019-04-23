namespace Macabre2D.Framework.Diagnostics {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base class for drawing the outlines of components.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseComponent"/>
    /// <seealso cref="Macabre2D.Framework.IDrawableComponent"/>
    public abstract class BaseDrawer : BaseComponent, IDrawableComponent {

        /// <inheritdoc/>
        public abstract BoundingArea BoundingArea { get; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        /// <value>The line thickness.</value>
        [DataMember]
        public float LineThickness { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether this should use dynamic line thickness.
        /// </summary>
        /// <value><c>true</c> if this should use dynamic line thickness; otherwise, <c>false</c>.</value>
        public bool UseDynamicLineThickness { get; set; }

        /// <summary>
        /// Gets the primitive drawer.
        /// </summary>
        /// <value>The primitive drawer.</value>
        protected PrimitiveDrawer PrimitiveDrawer { get; private set; }

        /// <inheritdoc/>
        public abstract void Draw(GameTime gameTime, float viewHeight);

        /// <summary>
        /// Gets the line thickness.
        /// </summary>
        /// <param name="viewHeight">Height of the view.</param>
        /// <returns>The appropriate line thickness for this drawer.</returns>
        protected float GetLineThickness(float viewHeight) {
            var result = this.LineThickness;
            if (this.UseDynamicLineThickness && this._scene?.Game != null) {
                result *= GameSettings.Instance.GetPixelAgnosticRatio(viewHeight, this._scene.Game.GraphicsDevice.Viewport.Height);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            if (this._scene != null) {
                this.PrimitiveDrawer = this._scene.ResolveDependency(
                    () => new PrimitiveDrawer(this._scene.Game.SpriteBatch));
            }
        }
    }
}