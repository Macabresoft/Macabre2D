namespace Macabre2D.UI.Editor;

using System.ComponentModel;
using System.Windows.Input;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabre2D.UI.Common;
using Macabresoft.AvaloniaEx;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for selecting gizmos.
/// </summary>
public class GizmoSelectionViewModel : BaseViewModel {
    private readonly IEntityService _entityService;
    private readonly ISceneService _sceneService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GizmoSelectionViewModel" /> class.
    /// </summary>
    public GizmoSelectionViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GizmoSelectionViewModel" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The entity service.</param>
    /// <param name="sceneService">The scene service.</param>
    [InjectionConstructor]
    public GizmoSelectionViewModel(IEditorService editorService, IEntityService entityService, ISceneService sceneService) {
        this.EditorService = editorService;
        this._entityService = entityService;
        this._sceneService = sceneService;

        this._entityService.PropertyChanged += this.EntityService_PropertyChanged;
        this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
        this.SetSelectedGizmoCommand = ReactiveCommand.Create<GizmoKind>(this.SetSelectedGizmo);
        this.ClearLayersCommand = ReactiveCommand.Create(this.ClearLayers);
        this.SelectAllLayersCommand = ReactiveCommand.Create(this.SelectAllLayers);
    }

    /// <summary>
    /// Gets a command to clear all layers from rendering.
    /// </summary>
    public ICommand ClearLayersCommand { get; }

    /// <summary>
    /// Gets the editor service.
    /// </summary>
    public IEditorService EditorService { get; }

    /// <summary>
    /// Gets a value indicating whether a scene is currently being edited.
    /// </summary>
    public bool IsScene => !this._sceneService.IsEditingPrefab;

    /// <summary>
    /// Gets a value indicating whether the selected entity is tileable.
    /// </summary>
    public bool IsTileable => this._entityService.Selected is ITileableEntity;

    /// <summary>
    /// Gets a command to select all layers to render.
    /// </summary>
    public ICommand SelectAllLayersCommand { get; }

    /// <summary>
    /// Gets a command to set the selected gizmo.
    /// </summary>
    public ICommand SetSelectedGizmoCommand { get; }

    private void ClearLayers() {
        if (EnumHelper.TryGetZero(typeof(Layers), out var result) && result is Layers layers) {
            this.EditorService.LayersToRender = layers;
        }
    }

    private void EntityService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEntityService.Selected)) {
            this.RaisePropertyChanged(nameof(this.IsTileable));

            if (this._entityService.Selected != null) {
                if (this.IsTileable) {
                    this.EditorService.SelectedGizmo = GizmoKind.Tile;
                }
                else if (this.EditorService.SelectedGizmo == GizmoKind.Tile && !this.IsTileable) {
                    this.EditorService.SelectedGizmo = GizmoKind.Translation;
                }
            }
        }
    }

    private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ISceneService.IsEditingPrefab)) {
            this.RaisePropertyChanged(nameof(this.IsScene));
        }
    }

    private void SelectAllLayers() {
        if (EnumHelper.TryGetAll(typeof(Layers), out var result) && result is Layers layers) {
            this.EditorService.LayersToRender = layers;
        }
    }

    private void SetSelectedGizmo(GizmoKind kind) {
        this.EditorService.SelectedGizmo = kind;
    }
}