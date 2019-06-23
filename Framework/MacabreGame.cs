﻿namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MacabreGame : Game, IGame {
        protected readonly GraphicsDeviceManager _graphics;
        protected bool _isLoaded;
        protected SpriteBatch _spriteBatch;
        private static IGame _instance;
        private IAssetManager _assetManager = new AssetManager();
        private IScene _currentScene;
        private bool _isInitialized;
        private IGameSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacabreGame"/> class.
        /// </summary>
        public MacabreGame() {
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.Settings = new GameSettings();
            MacabreGame.Instance = this;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="IGame"/> for the current session.
        /// </summary>
        /// <remarks>This is internal so it can be mocked for a test.</remarks>
        /// <value>The instance.</value>
        public static IGame Instance {
            get {
                return MacabreGame._instance;
            }

            internal set {
                if (value != null) {
                    MacabreGame._instance = value;
                }
            }
        }

        /// <inheritdoc/>
        public IAssetManager AssetManager {
            get {
                return this._assetManager;
            }

            set {
                if (value != null) {
                    this._assetManager = value;

                    if (this.Content != null) {
                        this._assetManager.Initialize(this.Content);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IScene CurrentScene {
            get {
                return this._currentScene;
            }

            set {
                if (value == null) {
                    throw new NotSupportedException($"{nameof(this.CurrentScene)} cannot be null!");
                }

                if (this._currentScene != value) {
                    this._currentScene = value;

                    if (this._isInitialized) {
                        this._currentScene.Initialize();
                    }

                    if (this._isLoaded) {
                        this._currentScene.LoadContent();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IGameSettings Settings {
            get {
                return this._settings;
            }

            set {
                if (value != null) {
                    this._settings = value;
                    GameSettings.Instance = this._settings;
                }
            }
        }

        /// <inheritdoc/>
        public SpriteBatch SpriteBatch {
            get {
                return this._spriteBatch;
            }
        }

        /// <inheritdoc/>
        protected override void Draw(GameTime gameTime) {
            if (this.CurrentScene != null) {
                this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
                this.CurrentScene.Draw(gameTime);
            }
            else {
                this.GraphicsDevice.Clear(this.Settings.FallbackBackgroundColor);
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            base.Initialize();
            this.CurrentScene?.Initialize();
            this._isInitialized = true;
        }

        /// <inheritdoc/>
        protected override void LoadContent() {
            this.AssetManager.Load<AssetManager>(Framework.AssetManager.ContentFileName);
            this.AssetManager.Initialize(this.Content);
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Settings = this.AssetManager.Load<GameSettings>(GameSettings.ContentFileName);
            this.CurrentScene = this.AssetManager.Load<Scene>(this.Settings.StartupSceneContentId);
            this.CurrentScene?.LoadContent();
            this._isLoaded = true;
        }

        /// <inheritdoc/>
        protected override void UnloadContent() {
            this.Content.Unload();
        }

        /// <inheritdoc/>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                //this.Exit();
            }

            this.CurrentScene?.Update(gameTime);
        }
    }
}