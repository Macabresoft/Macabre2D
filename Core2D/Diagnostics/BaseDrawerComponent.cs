namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base class for drawing the outlines of components.
    /// </summary>
    public abstract class BaseDrawerComponent : GameRenderableComponent {
        private Color _color = Color.White;
        private float _lineThickness = 1f;
        private bool _useDynamicLineThickness;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 0)]
        public Color Color {
            get {
                return this._color;
            }

            set {
                this.Set(ref this._color, value);
            }
        }

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        /// <value>The line thickness.</value>
        [DataMember(Order = 1)]
        public float LineThickness {
            get {
                return this._lineThickness;
            }

            set {
                this.Set(ref this._lineThickness, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this should use dynamic line thickness.
        /// </summary>
        /// <value><c>true</c> if this should use dynamic line thickness; otherwise, <c>false</c>.</value>
        [DataMember(Order = 2)]
        public bool UseDynamicLineThickness {
            get {
                return this._useDynamicLineThickness;
            }

            set {
                this.Set(ref this._useDynamicLineThickness, value);
            }
        }

        /// <summary>
        /// Gets the primitive drawer.
        /// </summary>
        /// <value>The primitive drawer.</value>
        protected PrimitiveDrawer? PrimitiveDrawer { get; private set; }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

#nullable disable
            this.PrimitiveDrawer = this.Entity.Scene.ResolveDependency(
                () => new PrimitiveDrawer(this.Entity.Scene.Game.SpriteBatch));
#nullable enable
        }

        /// <summary>
        /// Gets the line thickness.
        /// </summary>
        /// <param name="viewHeight">Height of the view.</param>
        /// <returns>The appropriate line thickness for this drawer.</returns>
        protected float GetLineThickness(float viewHeight) {
            var result = this.LineThickness;
            if (this.UseDynamicLineThickness && this.Entity.Scene.Game.GraphicsDevice is GraphicsDevice device) {
                result *= GameSettings.Instance.GetPixelAgnosticRatio(viewHeight, device.Viewport.Height);
            }

            return result;
        }
    }
}