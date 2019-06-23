namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An empty game that is referenced if no <see cref="MacabreGame"/> haas been created.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IGame"/>
    internal class EmptyGame : IGame {

        /// <inheritdoc/>
        public IAssetManager AssetManager {
            get {
                return default;
            }
        }

        /// <inheritdoc/>
        public ContentManager Content {
            get {
                return default;
            }
        }

        /// <inheritdoc/>
        public IScene CurrentScene {
            get {
                return EmptyScene.Instance;
            }
        }

        /// <inheritdoc/>
        public GraphicsDevice GraphicsDevice {
            get {
                return null;
            }
        }

        /// <inheritdoc/>
        public IGameSettings Settings {
            get {
                return GameSettings.Instance;
            }
        }

        /// <inheritdoc/>
        public SpriteBatch SpriteBatch {
            get {
                return default;
            }
        }
    }
}