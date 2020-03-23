namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// An empty game that is referenced if no <see cref="MacabreGame"/> haas been created.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IGame"/>
    internal class EmptyGame : IGame {

        /// <inheritdoc/>
        public event EventHandler<double> GameSpeedChanged;

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
        public double GameSpeed {
            get {
                return 1f;
            }

            set {
                this.GameSpeedChanged.SafeInvoke(this, 1f);
            }
        }

        /// <inheritdoc/>
        public GraphicsDevice GraphicsDevice {
            get {
                return null;
            }
        }

        /// <inheritdoc/>
        public GraphicsSettings GraphicsSettings { get; } = new GraphicsSettings();

        /// <inheritdoc/>
        public bool InstantiatePrefabs {
            get {
                return false;
            }
        }

        /// <inheritdoc/>
        public ISaveDataManager SaveDataManager { get; } = new EmptySaveDataManager();

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

        /// <inheritdoc/>
        public void SaveAndApplyGraphicsSettings() {
            return;
        }
    }
}