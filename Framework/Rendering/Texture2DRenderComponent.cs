namespace Macabresoft.Macabre2D.Framework {
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component that renders a <see cref="Texture2D"/> provided at runtime.
    /// </summary>
    public sealed class Texture2DRenderComponent : BaseSpriteComponent {
        private Texture2D? _texture;

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        public Texture2D? Texture {
            get => this._texture;
            set {
                this._texture = value;
                if (this._texture != null) {
                    this.SpriteSheet.LoadContent(this._texture);
                }
                else {
                    this.SpriteSheet.UnloadContent();
                }
            }
        }

        /// <inheritdoc />
        protected override byte SpriteIndex => 0;

        /// <inheritdoc />
        protected override SpriteSheet SpriteSheet { get; } = new();
    }
}