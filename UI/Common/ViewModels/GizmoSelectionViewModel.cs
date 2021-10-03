namespace Macabresoft.Macabre2D.UI.Common {
    using System.ComponentModel;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for selecting gizmos.
    /// </summary>
    public class GizmoSelectionViewModel : BaseViewModel {
        private readonly IEntityService _entityService;

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
        [InjectionConstructor]
        public GizmoSelectionViewModel(IEditorService editorService, IEntityService entityService) {
            this.EditorService = editorService;
            this._entityService = entityService;

            this._entityService.PropertyChanged += this.EntityService_PropertyChanged;
            this.SetSelectedGizmoCommand = ReactiveCommand.Create<GizmoKind>(this.SetSelectedGizmo);
        }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets a value indicating whether or not the selected entity is rotatable.
        /// </summary>
        public bool IsRotatable => this._entityService.Selected is IRotatable;

        /// <summary>
        /// Gets a value indicating whether or not the selected entity is tileable.
        /// </summary>
        public bool IsTileable => this._entityService.Selected is ITileableEntity;

        /// <summary>
        /// Gets a command to set the selected gizmo.
        /// </summary>
        public ICommand SetSelectedGizmoCommand { get; }

        private void EntityService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEntityService.Selected)) {
                this.RaisePropertyChanged(nameof(this.IsRotatable));
                this.RaisePropertyChanged(nameof(this.IsTileable));

                if (this._entityService.Selected != null) {
                    if (this.IsTileable) {
                        this.EditorService.SelectedGizmo = GizmoKind.Tile;
                    }
                    else if (this.EditorService.SelectedGizmo == GizmoKind.Rotation && !this.IsRotatable ||
                             this.EditorService.SelectedGizmo == GizmoKind.Tile && !this.IsTileable) {
                        this.EditorService.SelectedGizmo = GizmoKind.Translation;
                    }
                }
            }
        }

        private void SetSelectedGizmo(GizmoKind kind) {
            this.EditorService.SelectedGizmo = kind;
        }
    }
}