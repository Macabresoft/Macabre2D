namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for interacting with the editor and its many gizmos.
    /// </summary>
    public interface IEditorService : INotifyPropertyChanged {
        /// <summary>
        /// Gets or sets the drop shadow color.
        /// </summary>
        Color DropShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the selected gizmo.
        /// </summary>
        GizmoKind SelectedGizmo { get; set; }

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
        private Color _dropShadowColor = DefinedColors.MacabresoftBlack * 0.4f;
        private GizmoKind _selectedGizmo = GizmoKind.Translation;
        private Color _selectionColor = DefinedColors.MacabresoftYellow;
        private bool _showGrid = true;
        private Color _xAxisColor = DefinedColors.ZvukostiGreen;
        private Color _yAxisColor = DefinedColors.MacabresoftRed;

        /// <inheritdoc />
        public Color DropShadowColor {
            get => this._dropShadowColor;
            set => this.RaiseAndSetIfChanged(ref this._dropShadowColor, value);
        }

        /// <inheritdoc />
        public GizmoKind SelectedGizmo {
            get => this._selectedGizmo;
            set => this.RaiseAndSetIfChanged(ref this._selectedGizmo, value);
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
    }
}