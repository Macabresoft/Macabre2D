namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Microsoft.Xna.Framework;
using Unity;

/// <summary>
/// A view model that holds a <see cref="IEditorGame" /> and handles interactions with it from
/// the view.
/// </summary>
public sealed class SceneEditorViewModel : BaseViewModel {
    private readonly IEditorService _editorService;
    private readonly IEntityService _entityService;
    private readonly IEditorGame _game;
    private readonly ILoopService _loopService;
    private readonly IScene _scene;
    private readonly IEditorSettingsService _settingsService;
    private readonly IUndoService _undoService;
    private ICamera _camera;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    public SceneEditorViewModel() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The entity service.</param>
    /// <param name="game">The game.</param>
    /// <param name="loopService">The loop service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="undoService">The undo service.</param>
    [InjectionConstructor]
    public SceneEditorViewModel(
        IEditorService editorService,
        IEntityService entityService,
        IEditorGame game,
        ILoopService loopService,
        ISceneService sceneService,
        IEditorSettingsService settingsService,
        IUndoService undoService) : base() {
        this._editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        this._entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
        this._game = game ?? throw new ArgumentNullException(nameof(game));
        this._loopService = loopService;
        this.SceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
        this._settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        this._undoService = undoService ?? throw new ArgumentNullException(nameof(undoService));

        this._scene = this.CreateScene();
        this.TryLoadScene();
        this._camera.PropertyChanged += this.Camera_PropertyChanged;

        this._editorService.CenterCameraRequested += this.EditorService_CenterCameraRequested;
        this._editorService.FocusRequested += this.EditorService_FocusRequested;
        this._editorService.ZoomInRequested += this.EditorService_ZoomInRequested;
        this._editorService.ZoomOutRequested += this.EditorService_ZoomOutRequested;
        this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
        this.SceneService.PropertyChanged += this.SceneService_PropertyChanged;
    }

    /// <summary>
    /// Gets the scene service.
    /// </summary>
    public ISceneService SceneService { get; }

    private void Camera_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(ICamera.Transform):
                this._settingsService.Settings.CameraPosition = this._camera.Transform.Position;
                break;
            case nameof(ICamera.ActualViewHeight):
                this._settingsService.Settings.CameraViewHeight = this._camera.ActualViewHeight;
                break;
        }
    }

    private IScene CreateScene() {
        var scene = new Scene();
        scene.AddLoop(new EditorRenderLoop(this.SceneService));
        var camera = scene.AddChild<Camera>();
        camera.PixelSnap = PixelSnap.No;
        this._camera = camera;
        this._camera.ViewHeight = this._settingsService.Settings.CameraViewHeight;
        this._camera.SetWorldPosition(this._settingsService.Settings.CameraPosition);
        this._camera.AddChild(new CameraController(this._editorService));
        this._camera.AddChild(new EditorGrid(this._editorService, this._entityService));
        this._camera.AddChild(new SelectionDisplay(this._editorService, this._entityService, this._loopService));
        var selectorGizmo = new SelectorGizmo(this.SceneService);
        this._camera.AddChild(selectorGizmo);

        var translationGizmo = new TranslationGizmo(this._editorService, this.SceneService, this._entityService, this._undoService);
        this._camera.AddChild(translationGizmo);

        var scaleGizmo = new ScaleGizmo(this._editorService, this.SceneService, this._entityService, this._undoService);
        this._camera.AddChild(scaleGizmo);

        var rotationGizmo = new RotationGizmo(this._editorService, this._entityService, this.SceneService, this._undoService);
        this._camera.AddChild(rotationGizmo);

        var tileGizmo = new TileGizmo(this._entityService, this._undoService);
        this._camera.AddChild(tileGizmo);

        if (this._editorService.SelectedGizmo == GizmoKind.Tile && this._entityService.Selected is not ITileableEntity) {
            this._editorService.SelectedGizmo = GizmoKind.Selector;
        }

        scene.AddLoop(new EditorUpdateLoop(this._entityService, this.SceneService, selectorGizmo));
        return scene;
    }

    private void EditorService_CenterCameraRequested(object sender, EventArgs e) {
        this._camera.LocalPosition = Vector2.Zero;
    }

    private void EditorService_FocusRequested(object sender, IEntity e) {
        if (e != null) {
            this._camera.LocalPosition = e.Transform.Position;
        }
    }

    private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEditorService.SelectedTab) && this._editorService.SelectedTab == EditorTabs.Scene) {
            this.TryLoadScene();
        }
    }

    private void EditorService_ZoomInRequested(object sender, EventArgs e) {
        this._camera.ViewHeight *= 0.75f;
    }

    private void EditorService_ZoomOutRequested(object sender, EventArgs e) {
        this._camera.ViewHeight *= 1.25f;
    }

    private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ISceneService.CurrentScene) && !Scene.IsNullOrEmpty(this.SceneService.CurrentScene)) {
            this.SceneService.CurrentScene.Initialize(this._game, this._game.Assets);
        }
    }

    private void TryLoadScene() {
        if (this._editorService.SelectedTab == EditorTabs.Scene) {
            this._game.LoadScene(this._scene);
        }
    }
}