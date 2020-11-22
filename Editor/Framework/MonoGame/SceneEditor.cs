namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {
    using System.Collections.Generic;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Components;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Systems;

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
        
        /// <summary>
        /// Gets the selector gizmo.
        /// </summary>
        public IGizmo SelectorGizmo { get; }
        
        /// <summary>
        /// Gets the translation gizmo.
        /// </summary>
        public IGizmo TranslationGizmo { get; }
    }

    /// <summary>
    /// An implementation of <see cref="AvaloniaGame" /> used for editing a scene inside the
    /// Macabre2D editor.
    /// </summary>
    public class SceneEditor : AvaloniaGame, ISceneEditor {
        private readonly IUndoService _undoService;
        private readonly IEditorService _editorService;
        private readonly ISceneService _sceneService;
        private readonly IEntitySelectionService _selectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditor" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="selectionService">The selection service</param>
        /// <param name="undoService">The undo service.</param>
        public SceneEditor(
            IEditorService editorService,
            ISceneService sceneService, 
            IEntitySelectionService selectionService,
            IUndoService undoService) {
            this._editorService = editorService;
            this._sceneService = sceneService;
            this._selectionService = selectionService;
            this._undoService = undoService;
            
            // TODO: remove the following code once scene loading exists
            this._sceneService.CreateNewScene<GameScene>();
            this._sceneService.CurrentScene.BackgroundColor = DefinedColors.MacabresoftPurple;
        }

        /// <inheritdoc />
        public ICameraComponent Camera { get; private set; }

        /// <inheritdoc />
        public IGizmo SelectorGizmo { get; private set; }
        
        /// <inheritdoc />
        public IGizmo TranslationGizmo { get; private set; }


        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                    this.GraphicsDevice.Clear(this._sceneService.CurrentScene.BackgroundColor);
                    this.Scene.Render(this.FrameTime, this.InputState);
                }
                else {
                    this.GraphicsDevice.Clear(DefinedColors.MacabresoftBlack);
                }
            }
        }

        /// <inheritdoc />
        protected override void Initialize() {
            this.LoadScene(this.CreateScene());

            base.Initialize();

            if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                var circleEntity = this._sceneService.CurrentScene.AddChild();
                circleEntity.LocalPosition += Vector2.One;

                this._sceneService.CurrentScene.Initialize(this);
                
                // TODO: remove the following code once scene loading exists
                var circleBody = circleEntity.AddComponent<SimplePhysicsBodyComponent>();
                circleBody.Collider = new CircleCollider() {
                    Radius = 2f,
                };

                var circleDrawer = circleEntity.AddComponent<ColliderDrawerComponent>();
                circleDrawer.Update(new FrameTime(), new InputState());
                circleDrawer.LineThickness = 5f;
            }

            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
        }

        private IGameScene CreateScene() {
            var scene = new GameScene();
            scene.AddSystem(new EditorRenderSystem(this._sceneService));
            scene.AddSystem(new EditorUpdateSystem(this._editorService));
            var cameraEntity = scene.AddChild();
            this.Camera = cameraEntity.AddComponent<CameraComponent>();
            cameraEntity.AddComponent<CameraControlComponent>();
            cameraEntity.AddComponent(new EditorGridComponent(this._editorService, this._sceneService));
            cameraEntity.AddComponent(new SelectionDisplayComponent(this._editorService, this._selectionService));
            var selectorGizmo = new SelectorComponent(this._sceneService, this._selectionService);
            cameraEntity.AddComponent(selectorGizmo);
            this.SelectorGizmo = selectorGizmo;
            var translationGizmoEntity = cameraEntity.AddChild();
            var translationGizmo = new TranslationGizmoComponent(this._editorService, this._selectionService, this._undoService);
            translationGizmoEntity.AddComponent(translationGizmo);
            this.TranslationGizmo = translationGizmo;
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