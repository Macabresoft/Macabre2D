namespace Macabresoft.Macabre2D.Editor.Framework.MonoGame {

    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An implementation of <see cref="AvaloniaGame" /> used for editing a scene inside the
    /// Macabre2D editor.
    /// </summary>
    public class SceneEditorGame : AvaloniaGame, ISceneEditor {
        private IGameScene _sceneToEdit = GameScene.Empty;

        /// <inheritdoc />
        public IGameCameraComponent Camera { get; private set; }

        /// <inheritdoc />
        public IGameScene SceneToEdit {
            get => this._sceneToEdit;
            set {
                if (GameScene.IsNullOrEmpty(value)) {
                    this._sceneToEdit = GameScene.Empty;
                }
                else {
                    this._sceneToEdit = value;

                    if (this.IsInitialized) {
                        this._sceneToEdit.Initialize(this);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                if (!GameScene.IsNullOrEmpty(this._sceneToEdit)) {
                    this.GraphicsDevice.Clear(this.SceneToEdit.BackgroundColor);
                    this.Scene.Render(this.FrameTime, this.InputState);
                }
                else {
                    this.GraphicsDevice.Clear(Color.Black);
                }
            }
        }

        /// <inheritdoc />
        protected override void Initialize() {
            this.LoadScene(this.CreateScene());

            base.Initialize();

            if (!GameScene.IsNullOrEmpty(this._sceneToEdit)) {
                this._sceneToEdit.Initialize(this);
            }
        }

        private IGameScene CreateScene() {
            var scene = new GameScene();
            scene.AddSystem<EditorRenderSystem>();
            scene.AddSystem<UpdateSystem>();
            this.Camera = scene.AddChild().AddComponent<CameraComponent>();
            return scene;
        }
    }
}