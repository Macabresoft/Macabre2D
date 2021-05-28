namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Entities;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Systems;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An extension of <see cref="IAvaloniaGame" /> that makes editing a Macabre2D
    /// <see cref="IScene" />
    /// easier.
    /// </summary>
    public interface ISceneEditor : IAvaloniaGame {
        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public ICamera Camera { get; }

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
        private readonly ISceneService _sceneService;
        private readonly ISelectionService _selectionService;
        private readonly IUndoService _undoService;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditor" /> class.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="editorService">The editor service.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="projectService">The project service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="selectionService">The selection service</param>
        /// <param name="undoService">The undo service.</param>
        public SceneEditor(
            IAssetManager assetManager,
            IEditorService editorService,
            IPathService pathService,
            IProjectService projectService,
            ISceneService sceneService,
            ISelectionService selectionService,
            IUndoService undoService) : base(assetManager) {
            this._editorService = editorService;
            this._projectService = projectService;
            this._sceneService = sceneService;
            this._selectionService = selectionService;
            this._undoService = undoService;

            this.Content.RootDirectory = Path.GetRelativePath(pathService.EditorBinDirectoryPath, pathService.EditorContentDirectoryPath);
        }

        /// <inheritdoc />
        public IGizmo SelectedGizmo => this._gizmos.FirstOrDefault(x => x.GizmoKind == this._editorService.SelectedGizmo);

        /// <inheritdoc />
        public ICamera Camera { get; private set; }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime) {
            if (this.GraphicsDevice != null) {
                if (!Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
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
            if (!this._isInitialized) {
                try {
                    this.LoadScene(this.CreateScene());
                    base.Initialize();

                    if (!Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                        this._sceneService.CurrentScene.Initialize(this, this.CreateSceneLevelAssetManager());
                    }

                    this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
                    this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
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

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        private IScene CreateScene() {
            var scene = new Scene();
            scene.AddSystem(new EditorRenderSystem(this._sceneService));
            this.Camera = scene.AddChild<Camera>();
            this.Camera.AddChild<CameraController>();
            this.Camera.AddChild(new EditorGrid(this._editorService, this._sceneService));
            this.Camera.AddChild(new SelectionDisplay(this._editorService, this._selectionService));
            var selectorGizmo = new SelectorComponent(this._sceneService, this._selectionService);
            this.Camera.AddChild(selectorGizmo);

            var translationGizmo = new TranslationGizmo(this._editorService, this._selectionService, this._undoService);
            this.Camera.AddChild(translationGizmo);
            this._gizmos.Add(translationGizmo);

            var scaleGizmo = new ScaleGizmo(this._editorService, this._selectionService, this._undoService);
            this.Camera.AddChild(scaleGizmo);
            this._gizmos.Add(scaleGizmo);

            scene.AddSystem(new EditorUpdateSystem(this._editorService, selectorGizmo));
            return scene;
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (this.IsInitialized && e.PropertyName == nameof(IProjectService.CurrentProject)) {
                this.Project = this._projectService.CurrentProject;
            }
        }

        private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (this.IsInitialized &&
                e.PropertyName == nameof(ISceneService.CurrentScene) &&
                !Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this._sceneService.CurrentScene.Initialize(this, this.CreateSceneLevelAssetManager());
            }
        }
    }
}