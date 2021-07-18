namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for a type selection dialog.
    /// </summary>
    public class TypeSelectionViewModel : BaseDialogViewModel {
        private readonly List<Type> _types = new();
        private Type _selectedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public TypeSelectionViewModel() : base() {
            this.IsOkEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <param name="types">The types to select from.</param>
        [InjectionConstructor]
        public TypeSelectionViewModel(IEnumerable<Type> types) : this() {
            this._types.AddRange(types.OrderBy(x => x.FullName));
        }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        public IReadOnlyCollection<Type> Types => this._types;

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public Type SelectedType {
            get => this._selectedType;
            set {
                this.RaiseAndSetIfChanged(ref this._selectedType, value);
                this.IsOkEnabled = this.SelectedType != null;
            }
        }
    }
}