﻿namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using ReactiveUI;

    /// <summary>
    /// An interface for a generic service which handles the selection and loading of objects and their editors.
    /// </summary>
    public interface ISelectionService<T> : INotifyPropertyChanged where T : class {
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
        /// Gets or sets the selected object.
        /// </summary>
        T Selected { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>A task.</returns>
        void Initialize();
    }

    public abstract class SelectionService<T> : ReactiveObject, ISelectionService<T> where T : class {
        private readonly IAssemblyService _assemblyService;
        private readonly ObservableCollectionExtended<Type> _availableTypes = new();
        private readonly ObservableCollectionExtended<ValueEditorCollection> _editors = new();
        private readonly object _editorsLock = new();

        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;
        private bool _isBusy;
        private T _selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionService{T}" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        protected SelectionService(
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
        public T Selected {
            get => this._selected;
            set {
                if (this._selected != value) {
                    this.RaiseAndSetIfChanged(ref this._selected, value);

                    if (this.ShouldLoadEditors()) {
                        this.ResetEditors();
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Initialize() {
            var types = this.GetAvailableTypes(this._assemblyService);
            this._availableTypes.Reset(types.OrderBy(x => x.Name));
        }

        /// <summary>
        /// Gets a value indicating whether or not editors should be loaded.
        /// </summary>
        /// <returns>A value indicating whether or not editors should be loaded.</returns>
        protected virtual bool ShouldLoadEditors() {
            return this.Selected != null;
        }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <returns>The available types.</returns>
        protected abstract IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService);

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
            if (this.ShouldLoadEditors()) {
                lock (this._editorsLock) {
                    try {
                        Dispatcher.UIThread.Post(() => this.IsBusy = true);

                        var editorCollections = this._valueEditorService.CreateEditors(this.Selected).ToList();

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