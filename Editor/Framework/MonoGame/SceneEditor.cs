namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {

    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System.ComponentModel;

    /// <summary>
    /// An extension of <see cref="IAvaloniaGame" /> that makes editing a Macabre2D <see
    /// cref="IGameScene" /> easier.
    /// </summary>
    public interface ISceneEditor : IAvaloniaGame {

        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public ICameraComponent Camera { get; }
    }

    /// <summary>
    /// An implementation of <see cref="AvaloniaGame" /> used for editing a scene inside the
    /// Macabre2D editor.
    /// </summary>
    public class SceneEditor : AvaloniaGame, ISceneEditor {
        private readonly ISceneService _sceneService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditor" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        public SceneEditor(ISceneService sceneService) {
            this._sceneService = sceneService;
        }

        /// <inheritdoc />
        public ICameraComponent Camera { get; private set; }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                    this.GraphicsDevice.Clear(this._sceneService.CurrentScene.BackgroundColor);
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

            if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this._sceneService.CurrentScene.Initialize(this);
            }

            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
        }

        private IGameScene CreateScene() {
            var scene = new GameScene();
            scene.AddSystem(new EditorRenderSystem(this._sceneService));
            scene.AddSystem<UpdateSystem>();
            this.Camera = scene.AddChild().AddComponent<CameraComponent>();
            return scene;
        }

        private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISceneService.CurrentScene)) {
                if (this.IsInitialized && !GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                    this._sceneService.CurrentScene.Initialize(this);
                }
            }
        }
    }
}