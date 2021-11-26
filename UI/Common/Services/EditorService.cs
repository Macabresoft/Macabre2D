namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using Avalonia.Input;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
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
        /// Gets a command to request the camera to be centered on the scene.
        /// </summary>
        ICommand RequestCenterCameraCommand { get; }

        /// <summary>
        /// Gets a command to request focus of the currently selected entity.
        /// </summary>
        ICommand RequestFocusCommand { get; }

        /// <summary>
        /// Gets or sets the color of selected colliders.
        /// </summary>
        Color ColliderColor { get; set; }

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
        /// Gets or sets the selected gizmo.
        /// </summary>
        GizmoKind SelectedGizmo { get; set; }

        /// <summary>
        /// Gets or sets the cursor type.
        /// </summary>
        StandardCursorType CursorType { get; set; }

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        EditorTabs SelectedTab { get; set; }

        /// <summary>
        /// Gets or sets the selection color.
        /// </summary>
        Color SelectionColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the editor grid.
        /// </summary>
        bool ShowGrid { get; set; }

        /// <summary>
        /// Gets or sets the color of the x axis.
        /// </summary>
        Color XAxisColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the y axis.
        /// </summary>
        Color YAxisColor { get; set; }
    }

    /// <summary>
    /// A service for interacting with the editor and its many gizmos.
    /// </summary>
    public class EditorService : ReactiveObject, IEditorService {
        private readonly IEntityService _entityService;
        private readonly IEditorSettingsService _settingsService;

        private Color _colliderColor = DefinedColors.MacabresoftBone;
        private Color _dropShadowColor = DefinedColors.MacabresoftBlack * 0.4f;
        private byte _gridDivisions = 5;
        private Color _selectionColor = DefinedColors.MacabresoftYellow;
        private bool _showGrid = true;
        private Color _xAxisColor = DefinedColors.ZvukostiGreen;
        private Color _yAxisColor = DefinedColors.MacabresoftRed;
        private StandardCursorType _standardCursorType = StandardCursorType.None;

        /// <inheritdoc />
        public event EventHandler CenterCameraRequested;

        /// <inheritdoc />
        public event EventHandler<IEntity> FocusRequested;

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

            this.RequestCenterCameraCommand = ReactiveCommand.Create(this.RequestCenterCamera);
            this.RequestFocusCommand = ReactiveCommand.Create(
                this.RequestFocus,
                this._entityService.WhenAny(x => x.Selected, y => y.Value != null));
        }

        /// <inheritdoc />
        public ICommand RequestCenterCameraCommand { get; }

        /// <inheritdoc />
        public ICommand RequestFocusCommand { get; }

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
        public bool ShowGrid {
            get => this._showGrid;
            set => this.RaiseAndSetIfChanged(ref this._showGrid, value);
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
    }
}