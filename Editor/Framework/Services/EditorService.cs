namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using Framework;
    using Microsoft.Xna.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for interacting with the editor and its many gizmos.
    /// </summary>
    public interface IEditorService : INotifyPropertyChanged {
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
        
        /// <summary>
        /// Gets or sets the selection color.
        /// </summary>
        Color SelectionColor { get; set; }
    }

    /// <summary>
    /// A service for interacting with the editor and its many gizmos.
    /// </summary>
    public class EditorService : ReactiveObject, IEditorService {


        private Color _selectionColor = DefinedColors.MacabresoftYellow;
        private bool _showGrid = true;
        private Color _xAxisColor = DefinedColors.ZvukostiGreen;
        private Color _yAxisColor = DefinedColors.MacabresoftRed;

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

        /// <inheritdoc />
        public Color SelectionColor {
            get => this._selectionColor;
            set => this.RaiseAndSetIfChanged(ref this._selectionColor, value);
        }
    }
}