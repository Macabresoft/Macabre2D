namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using ReactiveUI;

    /// <summary>
    /// An interface for a generic service which handles the selection and loading of objects and their editors.
    /// </summary>
    public interface ISelectionService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the available entity types.
        /// </summary>
        IReadOnlyCollection<Type> AvailableTypes { get; }

        /// <summary>
        /// Gets the editors.
        /// </summary>
        IReadOnlyCollection<ValueControlCollection> Editors { get; }

        /// <summary>
        /// Gets a value indicating whether or not the value editor service is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>A task.</returns>
        void Initialize();
    }

    /// <summary>
    /// An interface for a generic service which handles the selection and loading of objects and their editors.
    /// </summary>
    public interface ISelectionService<T> : ISelectionService where T : class {
        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        T Selected { get; set; }
    }

    public abstract class SelectionService<T> : ReactiveObject, ISelectionService<T> where T : class {
        private readonly IAssemblyService _assemblyService;
        private readonly ObservableCollectionExtended<Type> _availableTypes = new();
        private readonly ObservableCollectionExtended<ValueControlCollection> _editors = new();
        private readonly object _editorsLock = new();

        private readonly IUndoService _undoService;
        private readonly IValueControlService _valueControlService;
        private bool _isBusy;
        private T _selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionService{T}" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueControlService">The value editor service.</param>
        protected SelectionService(
            IAssemblyService assemblyService,
            IUndoService undoService,
            IValueControlService valueControlService) {
            this._assemblyService = assemblyService;
            this._undoService = undoService;
            this._valueControlService = valueControlService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Type> AvailableTypes => this._availableTypes;

        /// <inheritdoc />
        public IReadOnlyCollection<ValueControlCollection> Editors {
            get {
                lock (this._editorsLock) {
                    return this._editors;
                }
            }
        }

        /// <inheritdoc />
        public bool IsBusy {
            get => this._isBusy;
            private set => this.RaiseAndSetIfChanged(ref this._isBusy, value);
        }

        /// <inheritdoc />
        public T Selected {
            get => this._selected;
            set {
                if (this._selected != value) {
                    this.RaiseAndSetIfChanged(ref this._selected, value);
                    this.ResetEditors();
                }
            }
        }

        /// <inheritdoc />
        public void Initialize() {
            var types = this.GetAvailableTypes(this._assemblyService);
            this._availableTypes.Reset(types.OrderBy(x => x.Name));
        }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <returns>The available types.</returns>
        protected abstract IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService);

        /// <summary>
        /// Gets a value indicating whether or not editors should be loaded.
        /// </summary>
        /// <returns>A value indicating whether or not editors should be loaded.</returns>
        protected virtual bool ShouldLoadEditors() {
            return this.Selected != null;
        }

        private void EditorCollection_OwnedValueChanged(object sender, ValueChangedEventArgs<object> e) {
            if (sender is IValueEditor { Owner: { } } valueEditor && !string.IsNullOrEmpty(valueEditor.ValuePropertyName)) {
                var originalValue = valueEditor.Owner.GetPropertyValue(valueEditor.ValuePropertyName);
                var newValue = e.UpdatedValue;

                if (originalValue != newValue) {
                    this._undoService.Do(() => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, newValue);
                        valueEditor.SetValue(newValue, true);
                    }, () => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, originalValue);
                        valueEditor.SetValue(originalValue, true);
                    });
                }
            }
        }

        private void ResetEditors() {
            if (this.ShouldLoadEditors()) {
                lock (this._editorsLock) {
                    try {
                        Dispatcher.UIThread.Post(() => this.IsBusy = true);

                        var editorCollections = this._valueControlService.CreateControls(this.Selected).ToList();

                        foreach (var editorCollection in this._editors) {
                            editorCollection.OwnedValueChanged -= this.EditorCollection_OwnedValueChanged;
                        }

                        foreach (var editorCollection in editorCollections) {
                            editorCollection.OwnedValueChanged += this.EditorCollection_OwnedValueChanged;
                        }

                        Dispatcher.UIThread.Post(() => this._editors.Reset(editorCollections));
                    }
                    finally {
                        Dispatcher.UIThread.Post(() => this.IsBusy = false);
                    }
                }
            }
            else {
                lock (this._editorsLock) {
                    Dispatcher.UIThread.Post(() => this._editors.Clear());
                }
            }
        }
    }
}