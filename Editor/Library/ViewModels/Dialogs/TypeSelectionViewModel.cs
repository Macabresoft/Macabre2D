namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels.Scene;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for a type selection dialog.
    /// </summary>
    public class TypeSelectionViewModel : ViewModelBase {
        /// <summary>
        /// An event that occurs when the dialog should close.
        /// </summary>
        public EventHandler<bool> CloseRequested;
        
        private readonly List<Type> _types = new();
        private Type _selectedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public TypeSelectionViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="baseType">The base type.</param>
        [InjectionConstructor]
        public TypeSelectionViewModel(IAssemblyService assemblyService, Type baseType) {
            var types = assemblyService.LoadTypes(baseType);
            this._types.AddRange(types.OrderBy(x => x.FullName));

            this.CancelCommand = ReactiveCommand.Create<Unit, Unit>(x => this.RequestClose(true));
            this.OkCommand = ReactiveCommand.Create<Unit, Unit>(
                x => this.RequestClose(false),
                this.WhenAny(x => x.SelectedType, y => y.Value != null));
        }

        /// <summary>
        /// A command which cancels the dialog.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// A command which provides the results of the dialog.
        /// </summary>
        public ICommand OkCommand { get; }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        public IReadOnlyCollection<Type> Types => this._types;

        // TODO: Validate
        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public Type SelectedType {
            get => this._selectedType;
            set => this.RaiseAndSetIfChanged(ref this._selectedType, value);
        }

        private Unit RequestClose(bool isCancel) {
            this.CloseRequested.SafeInvoke(this, !isCancel);
            return Unit.Default;
        }
    }
}