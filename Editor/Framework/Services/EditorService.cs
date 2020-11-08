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
        /// Gets a value indicating whether [show grid].
        /// </summary>
        /// <value><c>true</c> if [show grid]; otherwise, <c>false</c>.</value>
        bool ShowGrid { get; }

        /// <summary>
        /// Gets the color of the x axis and y axis.
        /// </summary>
        /// <value>The color of the x axis and y axis.</value>
        Color AxisColor { get; }
    }

    /// <summary>
    /// A service for interacting with the editor and its many gizmos.
    /// </summary>
    public class EditorService : ReactiveObject, IEditorService {
        private Color _axisColor = DefinedColors.ZvukostiGreen;
        private bool _showGrid = true;

        /// <inheritdoc />
        public bool ShowGrid {
            get => this._showGrid;

            set => this.RaiseAndSetIfChanged(ref this._showGrid, value);
        }

        /// <inheritdoc />
        public Color AxisColor {
            get => this._axisColor;

            set => this.RaiseAndSetIfChanged(ref this._axisColor, value);
        }
    }
}