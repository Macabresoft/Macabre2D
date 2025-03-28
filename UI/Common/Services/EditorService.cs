namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Input;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using ReactiveUI;

/// <summary>
/// Interface for interacting with the editor and its many gizmos.
/// </summary>
public interface IEditorService : INotifyPropertyChanged {
    /// <summary>
    /// Occurs when camera centering is requested.
    /// </summary>
    event EventHandler CenterCameraRequested;

    /// <summary>
    /// Occurs when focus is requested for an entity.
    /// </summary>
    event EventHandler<IEntity> FocusRequested;

    /// <summary>
    /// Requests the editor camera to zoom in.
    /// </summary>
    public event EventHandler ZoomInRequested;

    /// <summary>
    /// Requests the editor camera to zoom out.
    /// </summary>
    public event EventHandler ZoomOutRequested;

    /// <summary>
    /// Gets a command to request the camera to be centered on the scene.
    /// </summary>
    ICommand RequestCenterCameraCommand { get; }

    /// <summary>
    /// Gets a command to request focus of the currently selected entity.
    /// </summary>
    ICommand RequestFocusCommand { get; }

    /// <summary>
    /// Gets a command to zoom in the camera.
    /// </summary>
    ICommand RequestZoomInCommand { get; }

    /// <summary>
    /// Gets a command to zoom out the camera.
    /// </summary>
    ICommand RequestZoomOutCommand { get; }

    /// <summary>
    /// Gets or sets the color of selected colliders.
    /// </summary>
    Color ColliderColor { get; set; }

    /// <summary>
    /// Gets or sets the cursor type.
    /// </summary>
    StandardCursorType CursorType { get; set; }

    /// <summary>
    /// Gets or sets the drop shadow color.
    /// </summary>
    Color DropShadowColor { get; set; }

    /// <summary>
    /// Gets or sets the number of divisions between major grid lines.
    /// </summary>
    /// <value>The number of divisions.</value>
    byte GridDivisions { get; set; }

    /// <summary>
    /// Gets or sets the input device.
    /// </summary>
    InputDevice InputDeviceDisplay { get; set; }

    /// <summary>
    /// Gets or sets the layers to be rendered.
    /// </summary>
    Layers LayersToRender { get; set; }

    /// <summary>
    /// Gets or sets the selected gizmo.
    /// </summary>
    GizmoKind SelectedGizmo { get; set; }

    /// <summary>
    /// Gets or sets the selected tab.
    /// </summary>
    EditorTabs SelectedTab { get; set; }

    /// <summary>
    /// Gets or sets the selection color.
    /// </summary>
    Color SelectionColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show active tiles.
    /// </summary>
    bool ShowActiveTiles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the editor grid.
    /// </summary>
    bool ShowGrid { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to show all bounding areas and colliders.
    /// </summary>
    bool ShowBoundingAreasAndColliders { get; set; }

    /// <summary>
    /// Gets or sets the color of the x-axis.
    /// </summary>
    Color XAxisColor { get; set; }

    /// <summary>
    /// Gets or sets the color of the y-axis.
    /// </summary>
    Color YAxisColor { get; set; }
}

/// <summary>
/// A service for interacting with the editor and its many gizmos.
/// </summary>
public class EditorService : ReactiveObject, IEditorService {
    private readonly IEntityService _entityService;
    private readonly IEditorSettingsService _settingsService;
    private Color _colliderColor = Color.White;
    private Color _dropShadowColor = Color.Black * 0.4f;
    private byte _gridDivisions = 5;
    private Layers _layersToRender;
    private Color _selectionColor = new(200, 171, 55);
    private bool _showActiveTiles = true;
    private bool _showGrid = true;
    private bool _showBoundingAreasAndColliders;
    private StandardCursorType _standardCursorType = StandardCursorType.None;
    private Color _xAxisColor = new(113, 237, 100);
    private Color _yAxisColor = new(130, 38, 38);

    /// <inheritdoc />
    public event EventHandler CenterCameraRequested;

    /// <inheritdoc />
    public event EventHandler<IEntity> FocusRequested;

    /// <inheritdoc />
    public event EventHandler ZoomInRequested;

    /// <inheritdoc />
    public event EventHandler ZoomOutRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorService" /> class.
    /// </summary>
    /// <param name="entityService"></param>
    /// <param name="settingsService">The editor settings service.</param>
    public EditorService(
        IEntityService entityService,
        IEditorSettingsService settingsService) : base() {
        this._entityService = entityService;
        this._settingsService = settingsService;
        this._layersToRender = this._settingsService.Settings.LayersToRender;

        this.RequestCenterCameraCommand = ReactiveCommand.Create(this.RequestCenterCamera);
        this.RequestFocusCommand = ReactiveCommand.Create(
            this.RequestFocus,
            this._entityService.WhenAny(x => x.Selected, y => y.Value != null));

        this.RequestZoomInCommand = ReactiveCommand.Create(this.RequestZoomIn);
        this.RequestZoomOutCommand = ReactiveCommand.Create(this.RequestZoomOut);
    }

    /// <inheritdoc />
    public ICommand RequestCenterCameraCommand { get; }

    /// <inheritdoc />
    public ICommand RequestFocusCommand { get; }

    /// <inheritdoc />
    public ICommand RequestZoomInCommand { get; }

    /// <inheritdoc />
    public ICommand RequestZoomOutCommand { get; }

    /// <inheritdoc />
    public Color ColliderColor {
        get => this._colliderColor;
        set => this.RaiseAndSetIfChanged(ref this._colliderColor, value);
    }

    /// <inheritdoc />
    public StandardCursorType CursorType {
        get => this._standardCursorType;
        set => this.RaiseAndSetIfChanged(ref this._standardCursorType, value);
    }

    /// <inheritdoc />
    public Color DropShadowColor {
        get => this._dropShadowColor;
        set => this.RaiseAndSetIfChanged(ref this._dropShadowColor, value);
    }

    /// <inheritdoc />
    public byte GridDivisions {
        get => this._gridDivisions;
        set => this.RaiseAndSetIfChanged(ref this._gridDivisions, value);
    }

    /// <inheritdoc />
    public InputDevice InputDeviceDisplay {
        get => this._settingsService.Settings.InputDeviceDisplay;
        set {
            this._settingsService.Settings.InputDeviceDisplay = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public Layers LayersToRender {
        get => this._layersToRender;
        set {
            this.RaiseAndSetIfChanged(ref this._layersToRender, value);
            this._settingsService.Settings.LayersToRender = this._layersToRender;
        }
    }

    /// <inheritdoc />
    public GizmoKind SelectedGizmo {
        get => this._settingsService.Settings.LastGizmoSelected;
        set {
            this._settingsService.Settings.LastGizmoSelected = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public EditorTabs SelectedTab {
        get => this._settingsService.Settings.LastTabSelected;
        set {
            this._settingsService.Settings.LastTabSelected = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public Color SelectionColor {
        get => this._selectionColor;
        set => this.RaiseAndSetIfChanged(ref this._selectionColor, value);
    }

    /// <inheritdoc />
    public bool ShowActiveTiles {
        get => this._showActiveTiles;
        set => this.RaiseAndSetIfChanged(ref this._showActiveTiles, value);
    }

    /// <inheritdoc />
    public bool ShowGrid {
        get => this._showGrid;
        set => this.RaiseAndSetIfChanged(ref this._showGrid, value);
    }

    /// <inheritdoc />
    public bool ShowBoundingAreasAndColliders {
        get => this._showBoundingAreasAndColliders;
        set => this.RaiseAndSetIfChanged(ref this._showBoundingAreasAndColliders, value);
    }

    /// <inheritdoc />
    public Color XAxisColor {
        get => this._xAxisColor;
        set => this.RaiseAndSetIfChanged(ref this._xAxisColor, value);
    }

    /// <inheritdoc />
    public Color YAxisColor {
        get => this._yAxisColor;
        set => this.RaiseAndSetIfChanged(ref this._yAxisColor, value);
    }

    private void RequestCenterCamera() {
        this.CenterCameraRequested.SafeInvoke(this);
    }

    private void RequestFocus() {
        this.FocusRequested.SafeInvoke(this, this._entityService.Selected);
    }

    private void RequestZoomIn() {
        this.ZoomInRequested.SafeInvoke(this);
    }

    private void RequestZoomOut() {
        this.ZoomOutRequested.SafeInvoke(this);
    }
}