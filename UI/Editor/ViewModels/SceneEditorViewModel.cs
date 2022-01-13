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
    private readonly IScene _scene;
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
    /// <param name="game">The game.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="entityService">The entity service.</param>
    /// <param name="undoService">The undo service.</param>
    [InjectionConstructor]
    public SceneEditorViewModel(
        IEditorService editorService,
        IEditorGame game,
        ISceneService sceneService,
        IEntityService entityService,
        IUndoService undoService) : base() {
        this._editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        this._game = game ?? throw new ArgumentNullException(nameof(game));
        this.SceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
        this._entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
        this._undoService = undoService ?? throw new ArgumentNullException(nameof(undoService));

        this._scene = this.CreateScene();
        this.TryLoadScene();

        this._editorService.CenterCameraRequested += this.EditorService_CenterCameraRequested;
        this._editorService.FocusRequested += this.EntityService_FocusRequested;
        this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
        this.SceneService.PropertyChanged += this.SceneService_PropertyChanged;
    }

    /// <summary>
    /// Gets the scene service.
    /// </summary>
    public ISceneService SceneService { get; }

    private IScene CreateScene() {
        var scene = new Scene();
        scene.AddSystem(new EditorRenderSystem(this.SceneService));
        this._camera = scene.AddChild<Camera>();
        this._camera.AddChild(new CameraController(this._editorService));
        this._camera.AddChild(new EditorGrid(this._editorService, this._entityService));
        this._camera.AddChild(new SelectionDisplay(this._editorService, this._entityService));
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

        scene.AddSystem(new EditorUpdateSystem(this._entityService, selectorGizmo));
        return scene;
    }

    private void EditorService_CenterCameraRequested(object sender, EventArgs e) {
        this._camera.LocalPosition = Vector2.Zero;
    }

    private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEditorService.SelectedTab) && this._editorService.SelectedTab == EditorTabs.Scene) {
            this.TryLoadScene();
        }
    }

    private void EntityService_FocusRequested(object sender, IEntity e) {
        if (e != null) {
            this._camera.LocalPosition = e.Transform.Position;
        }
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