namespace Macabresoft.Macabre2D.UI.Common;

using System.ComponentModel;
using System.IO;
using System.Linq;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.AvaloniaInterop;
using Microsoft.Xna.Framework;

/// <summary>
/// An extension of <see cref="IAvaloniaGame" /> that makes editing a Macabre2D
/// <see cref="IScene" />
/// easier.
/// </summary>
public interface IEditorGame : IAvaloniaGame {
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
/// An implementation of <see cref="AvaloniaGame" /> used inside the Macabre2D editor.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EditorGame : AvaloniaGame, IEditorGame {
    private readonly IEditorService _editorService;
    private readonly IProjectService _projectService;
    private readonly ISceneService _sceneService;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorGame" /> class.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="editorService">The editor service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="projectService">The project service.</param>
    /// <param name="sceneService">The scene service.</param>
    public EditorGame(
        IAssetManager assetManager,
        IEditorService editorService,
        IPathService pathService,
        IProjectService projectService,
        ISceneService sceneService) : base(assetManager) {
        this._editorService = editorService;
        this._projectService = projectService;
        this._sceneService = sceneService;
        this.Content.RootDirectory = Path.GetRelativePath(pathService.EditorBinDirectoryPath, pathService.EditorContentDirectoryPath);
    }

    /// <inheritdoc />
    public ICamera Camera => this.CurrentScene.TryGetChild<ICamera>(out var camera) ? camera : null;

    /// <inheritdoc />
    public IGizmo SelectedGizmo { get; private set; }

    /// <inheritdoc />
    protected override void Draw(GameTime gameTime) {
        if (this.GraphicsDevice != null) {
            if (this.CurrentScene.Children.OfType<IScene>().Any() && Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this.CurrentScene.BackgroundColor = this._sceneService.CurrentScene.BackgroundColor;
            }

            this.GraphicsDevice.Clear(this.CurrentScene.BackgroundColor);
            this.CurrentScene.Render(this.FrameTime, this.InputState);
        }
    }

    /// <inheritdoc />
    protected override void Initialize() {
        if (!this._isInitialized) {
            try {
                base.Initialize();

                if (!Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                    this._sceneService.CurrentScene.Initialize(this, this.CreateAssetManager());
                }

                this.ResetGizmo();
                this.Camera.LayersToRender = this._editorService.LayersToRender;
                this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
                this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
                this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
                this.PropertyChanged += this.Self_PropertyChanged;
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

        this.TryCreateSpriteBatch();
        this.Assets.Initialize(this.Content, new Serializer());
    }

    private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEditorService.SelectedGizmo)) {
            this.ResetGizmo();
        }
        else if (e.PropertyName == nameof(IEditorService.LayersToRender)) {
            this.Camera.LayersToRender = this._editorService.LayersToRender;
        }
    }

    private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (this.IsInitialized && e.PropertyName == nameof(IProjectService.CurrentProject)) {
            this.Project = this._projectService.CurrentProject;
        }
    }

    private void ResetGizmo() {
        this.SelectedGizmo = this.CurrentScene.GetDescendants<IGizmo>().FirstOrDefault(x => x.GizmoKind == this._editorService.SelectedGizmo);
    }

    private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (this.IsInitialized &&
            e.PropertyName == nameof(ISceneService.CurrentScene) &&
            !Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
            this._sceneService.CurrentScene.Initialize(this, this.CreateAssetManager());
        }
    }

    private void Self_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.CurrentScene)) {
            this.ResetGizmo();
        }
    }
}