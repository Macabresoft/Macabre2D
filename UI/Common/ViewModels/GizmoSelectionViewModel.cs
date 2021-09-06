namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System.Windows.Input;
    using Macabresoft.Macabre2D.UI.Common.MonoGame;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for selecting gizmos.
    /// </summary>
    public class GizmoSelectionViewModel : BaseViewModel {
        /// <summary>
        /// Initializes a new instance of the <see cref="GizmoSelectionViewModel" /> class.
        /// </summary>
        public GizmoSelectionViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GizmoSelectionViewModel" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        [InjectionConstructor]
        public GizmoSelectionViewModel(IEditorService editorService) {
            this.EditorService = editorService;

            this.SetSelectedGizmoCommand = ReactiveCommand.Create<GizmoKind>(this.SetSelectedGizmo);
        }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets a command to set the selected gizmo.
        /// </summary>
        public ICommand SetSelectedGizmoCommand { get; }

        private void SetSelectedGizmo(GizmoKind kind) {
            this.EditorService.SelectedGizmo = kind;
        }
    }
}