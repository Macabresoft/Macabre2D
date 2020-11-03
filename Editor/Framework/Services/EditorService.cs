namespace Macabresoft.Macabre2D.Editor.Library.Services {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using ReactiveUI;
    using System.ComponentModel;

    /// <summary>
    /// Interface for interacting with the editor and its many gizmos.
    /// </summary>
    public interface IEditorService : INotifyPropertyChanged {

        /// <summary>
        /// Gets a value indicating whether [show grid].
        /// </summary>
        /// <value><c>true</c> if [show grid]; otherwise, <c>false</c>.</value>
        bool ShowGrid { get; }

        /// <summary>
        /// Gets the color of the x axis.
        /// </summary>
        /// <value>The color of the x axis.</value>
        Color XAxisColor { get; }

        /// <summary>
        /// Gets the color of the y axis.
        /// </summary>
        /// <value>The color of the y axis.</value>
        Color YAxisColor { get; }
    }

    /// <summary>
    /// A service for interacting with the editor and its many gizmos.
    /// </summary>
    public class EditorService : ReactiveObject, IEditorService {
        private bool _showGrid = true;
        private Color _xAxisColor = DefinedColors.ZvukostiGreen;
        private Color _yAxisColor = DefinedColors.MacabresoftRed;

        /// <inheritdoc />
        public bool ShowGrid {
            get {
                return this._showGrid;
            }

            set {
                this.RaiseAndSetIfChanged(ref this._showGrid, value);
            }
        }

        /// <inheritdoc />
        public Color XAxisColor {
            get {
                return this._xAxisColor;
            }

            set {
                this.RaiseAndSetIfChanged(ref this._xAxisColor, value);
            }
        }

        /// <inheritdoc />
        public Color YAxisColor {
            get {
                return this._yAxisColor;
            }

            set {
                this.RaiseAndSetIfChanged(ref this._yAxisColor, value);
            }
        }
    }
}