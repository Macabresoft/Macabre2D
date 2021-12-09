namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using ReactiveUI;

/// <summary>
/// An interface for a generic service which handles the selection and loading of objects and their editors.
/// </summary>
public interface ISelectionService : INotifyPropertyChanged {
    /// <summary>
    /// Gets the editors.
    /// </summary>
    IReadOnlyCollection<ValueControlCollection> Editors { get; }
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
    private readonly ObservableCollectionExtended<ValueControlCollection> _editors = new();
    private readonly object _editorsLock = new();

    private readonly IUndoService _undoService;
    private readonly IValueControlService _valueControlService;
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
        this.AssemblyService = assemblyService;
        this._undoService = undoService;
        this._valueControlService = valueControlService;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ValueControlCollection> Editors {
        get {
            lock (this._editorsLock) {
                return this._editors;
            }
        }
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

    /// <summary>
    /// Gets the assembly service.
    /// </summary>
    protected IAssemblyService AssemblyService { get; }

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

            this._undoService.Do(() => {
                valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, newValue);
                valueEditor.SetValue(newValue, true);
            }, () => {
                valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, originalValue);
                valueEditor.SetValue(originalValue, true);
            });
        }
    }

    private void ResetEditors() {
        if (this.ShouldLoadEditors()) {
            lock (this._editorsLock) {
                var editorCollections = this._valueControlService.CreateControls(this.Selected).ToList();

                foreach (var editorCollection in this._editors) {
                    editorCollection.OwnedValueChanged -= this.EditorCollection_OwnedValueChanged;
                }

                foreach (var editorCollection in editorCollections) {
                    editorCollection.OwnedValueChanged += this.EditorCollection_OwnedValueChanged;
                }

                Dispatcher.UIThread.Post(() => this._editors.Reset(editorCollections));
            }
        }
        else {
            lock (this._editorsLock) {
                Dispatcher.UIThread.Post(() => this._editors.Clear());
            }
        }
    }
}