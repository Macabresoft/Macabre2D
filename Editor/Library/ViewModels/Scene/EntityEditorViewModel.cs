namespace Macabresoft.Macabre2D.Editor.Library.ViewModels.Scene {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;
        private readonly ObservableCollectionExtended<ValueEditorCollection> _editors = new();
        private readonly object _editorsLock = new();
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorViewModel" /> class.
        /// </summary>
        public EntityEditorViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        [InjectionConstructor]
        public EntityEditorViewModel(
            IDialogService dialogService,
            ISceneService sceneService,
            ISelectionService selectionService,
            IUndoService undoService,
            IValueEditorService valueEditorService) {
            this._dialogService = dialogService;
            this._sceneService = sceneService;
            this.SelectionService = selectionService;
            this._undoService = undoService;
            this._valueEditorService = valueEditorService;
            this.SelectionService.PropertyChanged += this.SelectionService_PropertyChanged;
        }


        /// <summary>
        /// Gets the editors.
        /// </summary>
        public IReadOnlyCollection<ValueEditorCollection> Editors {
            get {
                lock (this._editorsLock) {
                    return this._editors;
                }
            }
        }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }

        private void EditorCollection_OwnedValueChanged(object sender, ValueChangedEventArgs<object> e) {
            if (sender is IValueEditor { Owner: { } } valueEditor && !string.IsNullOrEmpty(valueEditor.ValuePropertyName)) {
                var originalValue = valueEditor.Owner.GetPropertyValue(valueEditor.ValuePropertyName);
                var newValue = e.UpdatedValue;

                if (originalValue != newValue) {
                    var originalHasChanges = this._sceneService.HasChanges;
                    this._undoService.Do(() => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, newValue);
                        valueEditor.SetValue(newValue);
                        this._sceneService.HasChanges = true;
                    }, () => {
                        valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, originalValue);
                        valueEditor.SetValue(originalValue);
                        this._sceneService.HasChanges = originalHasChanges;
                    });
                }
            }
        }

        private void ResetComponentEditors() {
            if (this.SelectionService.SelectedEntity is IScene scene) {
            }
            else if (this.SelectionService.SelectedEntity is IEntity entity) {
                var editorCollections = this._valueEditorService.CreateEditors(entity).ToList();

                lock (this._editorsLock) {
                    foreach (var editorCollection in this._editors) {
                        editorCollection.OwnedValueChanged -= this.EditorCollection_OwnedValueChanged;
                    }

                    foreach (var editorCollection in editorCollections) {
                        editorCollection.OwnedValueChanged += this.EditorCollection_OwnedValueChanged;
                    }

                    this._editors.Reset(editorCollections);
                }
            }
        }

        private void SelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISelectionService.SelectedEntity)) {
                this.ResetComponentEditors();
            }
        }
    }
}