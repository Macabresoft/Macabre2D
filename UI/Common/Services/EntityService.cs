namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection and loading of entities and their editors.
    /// </summary>
    public interface IEntityService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the available entity types.
        /// </summary>
        IReadOnlyCollection<Type> AvailableTypes { get; }

        /// <summary>
        /// Gets the editors.
        /// </summary>
        IReadOnlyCollection<ValueEditorCollection> Editors { get; }

        /// <summary>
        /// Gets a value indicating whether or not the value editor service is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        IEntity SelectedEntity { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>A task.</returns>
        void Initialize();
    }

    /// <summary>
    /// A service which handles the selection and loading of entities and their editors.
    /// </summary>
    public sealed class EntityService : ReactiveObject, IEntityService {
        private readonly IAssemblyService _assemblyService;
        private readonly ObservableCollectionExtended<Type> _availableTypes = new();
        private readonly ObservableCollectionExtended<ValueEditorCollection> _editors = new();
        private readonly object _editorsLock = new();
        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;
        private bool _isBusy;
        private IEntity _selectedEntity;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        public EntityService(
            IAssemblyService assemblyService,
            IUndoService undoService,
            IValueEditorService valueEditorService) {
            this._assemblyService = assemblyService;
            this._undoService = undoService;
            this._valueEditorService = valueEditorService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Type> AvailableTypes => this._availableTypes;

        /// <inheritdoc />
        public IReadOnlyCollection<ValueEditorCollection> Editors {
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
        public IEntity SelectedEntity {
            get => this._selectedEntity;
            set {
                if (this._selectedEntity != value) {
                    this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
                    this.ResetEditors();
                }
            }
        }

        /// <inheritdoc />
        public void Initialize() {
            var types = this._assemblyService.LoadTypes(typeof(IEntity)).Where(x => !x.IsAssignableTo(typeof(IScene)));
            this._availableTypes.Reset(types.OrderBy(x => x.Name));
        }

        private void EditorCollection_OwnedValueChanged(object sender, ValueChangedEventArgs<object> e) {
            if (sender is IValueEditor { Owner: { } } valueEditor && !string.IsNullOrEmpty(valueEditor.ValuePropertyName)) {
                var originalValue = valueEditor.Owner.GetPropertyValue(valueEditor.ValuePropertyName);
                var newValue = e.UpdatedValue;

                if (originalValue != newValue) {
                    this._undoService.Do(() => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, newValue);
                        valueEditor.SetValue(newValue);
                    }, () => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, originalValue);
                        valueEditor.SetValue(originalValue);
                    });
                }
            }
        }

        private void ResetEditors() {
            if (this.SelectedEntity is IScene) {
            }
            else if (this.SelectedEntity is IEntity entity) {
                lock (this._editorsLock) {
                    try {
                        Dispatcher.UIThread.Post(() => this.IsBusy = true);

                        var editorCollections = this._valueEditorService.CreateEditors(entity).ToList();

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