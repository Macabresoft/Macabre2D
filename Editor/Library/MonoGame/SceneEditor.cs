namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Components;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Systems;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An extension of <see cref="IAvaloniaGame" /> that makes editing a Macabre2D
    /// <see cref="IGameScene" />
    /// easier.
    /// </summary>
    public interface ISceneEditor : IAvaloniaGame {
        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public ICameraComponent Camera { get; }

        /// <summary>
        /// Gets the selected gizmo.
        /// </summary>
        IGizmo SelectedGizmo { get; }
    }

    /// <summary>
    /// An implementation of <see cref="AvaloniaGame" /> used for editing a scene inside the
    /// Macabre2D editor.
    /// </summary>
    public class SceneEditor : AvaloniaGame, ISceneEditor {
        private readonly IEditorService _editorService;
        private readonly IList<IGizmo> _gizmos = new List<IGizmo>();
        private readonly IProjectService _projectService;
        private readonly ISelectionService _selectionService;
        private readonly IUndoService _undoService;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditor" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="projectService">The project service.</param>
        /// <param name="selectionService">The selection service</param>
        /// <param name="undoService">The undo service.</param>
        public SceneEditor(
            IEditorService editorService,
            IPathService pathService,
            IProjectService projectService,
            ISelectionService selectionService,
            IUndoService undoService) : base(projectService.Assets) {
            this._editorService = editorService;
            this._projectService = projectService;
            this._selectionService = selectionService;
            this._undoService = undoService;

            this.Content.RootDirectory = Path.GetRelativePath(pathService.EditorBinDirectoryPath, pathService.EditorContentDirectoryPath);
        }

        /// <inheritdoc />
        public IGizmo SelectedGizmo => this._gizmos.FirstOrDefault(x => x.GizmoKind == this._editorService.SelectedGizmo);

        /// <inheritdoc />
        public ICameraComponent Camera { get; private set; }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                if (!GameScene.IsNullOrEmpty(this._projectService.CurrentScene)) {
                    this.GraphicsDevice.Clear(this._projectService.CurrentScene.BackgroundColor);
                    this.Scene.Render(this.FrameTime, this.InputState);
                }
                else {
                    this.GraphicsDevice.Clear(DefinedColors.MacabresoftBlack);
                }
            }
        }

        /// <inheritdoc />
        protected override void Initialize() {
            if (!this._isInitialized) {
                try {
                    this.LoadScene(this.CreateScene());
                    base.Initialize();

                    if (!GameScene.IsNullOrEmpty(this._projectService.CurrentScene)) {
                        this._projectService.CurrentScene.Initialize(this);
                    }

                    this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
                }
                finally {
                    this._isInitialized = true;
                }
            }
        }

        /// <inheritdoc />
        protected override void LoadContent() {
            if (this._projectService.CurrentProject != null) {
                this.Project = this._projectService.CurrentProject;
            }

            if (this._projectService.Assets.TryLoadContent<GameScene>(this.Project.StartupSceneContentId, out var scene) && scene != null) {
                this._projectService.CurrentScene = scene;
                this._projectService.CurrentScene.Initialize(this);
            }

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        private IGameScene CreateScene() {
            var scene = new GameScene();
            scene.AddSystem(new EditorRenderSystem(this._projectService));
            var cameraEntity = scene.AddChild();
            this.Camera = cameraEntity.AddComponent<CameraComponent>();
            cameraEntity.AddComponent<CameraControlComponent>();
            cameraEntity.AddComponent(new EditorGridComponent(this._editorService, this._projectService));
            cameraEntity.AddComponent(new SelectionDisplayComponent(this._editorService, this._selectionService));
            var selectorGizmo = new SelectorComponent(this._projectService, this._selectionService);
            cameraEntity.AddComponent(selectorGizmo);

            var translationGizmoEntity = cameraEntity.AddChild();
            var translationGizmo = new TranslationGizmoComponent(this._editorService, this._selectionService, this._undoService);
            translationGizmoEntity.AddComponent(translationGizmo);
            this._gizmos.Add(translationGizmo);

            var scaleGizmoEntity = cameraEntity.AddChild();
            var scaleGizmo = new ScaleGizmoComponent(this._editorService, this._selectionService, this._undoService);
            scaleGizmoEntity.AddComponent(scaleGizmo);
            this._gizmos.Add(scaleGizmo);

            scene.AddSystem(new EditorUpdateSystem(this._editorService, selectorGizmo));
            return scene;
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (this.IsInitialized) {
                if (e.PropertyName == nameof(IProjectService.CurrentScene)) {
                    if (!GameScene.IsNullOrEmpty(this._projectService.CurrentScene)) {
                        this._projectService.CurrentScene.Initialize(this);
                    }
                }
                else if (e.PropertyName == nameof(IProjectService.CurrentProject)) {
                    this.Project = this._projectService.CurrentProject;
                }
            }
        }
    }
}