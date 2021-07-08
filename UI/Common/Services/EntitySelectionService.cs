namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection of entities and their components.
    /// </summary>
    public interface IEntitySelectionService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the editors.
        /// </summary>
        IReadOnlyCollection<ValueEditorCollection> Editors { get; }

        /// <summary>
        /// Gets a value indicating whether or not the value editor service is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets the most recently selected item
        /// </summary>
        EntitySelectionKind MostRecentlySelectedKind { get; }

        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        IEntity SelectedEntity { get; set; }

        /// <summary>
        /// Gets or sets the selected system.
        /// </summary>
        IUpdateableSystem SelectedSystem { get; set; }
    }

    /// <summary>
    /// A service which handles the selection of entities and their components.
    /// </summary>
    public sealed class EntityEntitySelectionService : ReactiveObject, IEntitySelectionService {
        private readonly ObservableCollectionExtended<ValueEditorCollection> _editors = new();
        private readonly object _editorsLock = new();
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;
        private bool _isBusy;
        private EntitySelectionKind _mostRecentlySelectedKind = EntitySelectionKind.None;
        private IEntity _selectedEntity;
        private IUpdateableSystem _selectedSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEntitySelectionService" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        public EntityEntitySelectionService(
            ISceneService sceneService,
            IUndoService undoService,
            IValueEditorService valueEditorService) {
            this._sceneService = sceneService;
            this._undoService = undoService;
            this._valueEditorService = valueEditorService;
        }

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
        public EntitySelectionKind MostRecentlySelectedKind {
            get => this._mostRecentlySelectedKind;
            private set => this.RaiseAndSetIfChanged(ref this._mostRecentlySelectedKind, value);
        }


        /// <inheritdoc />
        public IEntity SelectedEntity {
            get => this._selectedEntity;
            set {
                if (value is IScene scene) {
                    if (value != this._selectedEntity) {
                        this.SelectedSystem = scene.Systems.FirstOrDefault();
                    }

                    this.MostRecentlySelectedKind = EntitySelectionKind.Scene;
                }
                else if (value != null) {
                    this.MostRecentlySelectedKind = EntitySelectionKind.Entity;
                }
                else {
                    this.MostRecentlySelectedKind = EntitySelectionKind.None;
                    this.SelectedSystem = null;
                }

                if (this._selectedEntity != value) {
                    this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
                    this.ResetEditors();
                }
            }
        }

        /// <inheritdoc />
        public IUpdateableSystem SelectedSystem {
            get => this._selectedSystem;
            set {
                if (value != this._selectedSystem) {
                    this.RaiseAndSetIfChanged(ref this._selectedSystem, value);
                }

                if (this._selectedEntity != null) {
                    this._mostRecentlySelectedKind = this._selectedEntity is IScene ? EntitySelectionKind.Scene : EntitySelectionKind.Entity;
                }
                else if (this._mostRecentlySelectedKind == EntitySelectionKind.Entity || this._mostRecentlySelectedKind == EntitySelectionKind.Scene) {
                    this.MostRecentlySelectedKind = EntitySelectionKind.None;
                }
            }
        }

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
                    }, UndoScope.Scene);
                }
            }
        }

        private void ResetEditors() {
            if (this.SelectedEntity is IScene scene) {
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