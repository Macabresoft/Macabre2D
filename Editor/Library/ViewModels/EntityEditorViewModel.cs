namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {
        private readonly ReactiveCommand<Type, Unit> _addComponentCommand;
        private readonly ObservableCollectionExtended<ValueEditorCollection> _componentEditors = new();
        private readonly IDialogService _dialogService;
        private readonly object _editorsLock = new();
        private readonly ReactiveCommand<IGameComponent, Unit> _removeComponentCommand;
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
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        [InjectionConstructor]
        public EntityEditorViewModel(
            IDialogService dialogService,
            ISelectionService selectionService,
            IUndoService undoService,
            IValueEditorService valueEditorService) {
            this._dialogService = dialogService;
            this.SelectionService = selectionService;
            this._undoService = undoService;
            this._valueEditorService = valueEditorService;
            this.SelectionService.PropertyChanged += this.SelectionService_PropertyChanged;
            
            this._addComponentCommand = ReactiveCommand.CreateFromTask<Type>(
                async x => await this.AddComponent(x), 
                this.SelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null));
            
            this._removeComponentCommand = ReactiveCommand.Create<IGameComponent, Unit>(
                this.RemoveComponent,
                this.SelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null));
        }

        public ICommand AddComponentCommand => this._addComponentCommand;

        /// <summary>
        /// Gets the component editors.
        /// </summary>
        public IReadOnlyCollection<ValueEditorCollection> ComponentEditors {
            get {
                lock (this._editorsLock) {
                    return this._componentEditors;
                }
            }
        }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }

        private async Task AddComponent(Type type) {
            if (this.SelectionService.SelectedEntity is IGameEntity entity) {
                if (type == null) {
                    type = await this._dialogService.OpenTypeSelectionDialog(typeof(IGameComponent));
                }

                if (type != null) {
                    if (Activator.CreateInstance(type) is IGameComponent component) {
                        this._undoService.Do(() => {
                            Dispatcher.UIThread.Post(() => {
                                var valueEditorCollection = this._valueEditorService.GetComponentEditor(component, this._removeComponentCommand);
                                entity.AddComponent(component);
                                this._componentEditors.Add(valueEditorCollection);
                            });

                        }, () => {
                            Dispatcher.UIThread.Post(() => {
                                var valueEditorCollection = this._componentEditors.FirstOrDefault(x => x.Owner == component);
                                entity.RemoveComponent(component);
                                this._componentEditors.Remove(valueEditorCollection);
                            });
                        });
                    }
                }
            }
        }

        private void EditorCollection_OwnedValueChanged(object sender, ValueChangedEventArgs<object> e) {
            if (sender is IValueEditor valueEditor && valueEditor.Owner != null && !string.IsNullOrEmpty(valueEditor.ValuePropertyName)) {
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

        private Unit RemoveComponent(IGameComponent component) {
            if (component != null) {
                lock (this._editorsLock) {
                    var entity = component.Entity;
                    var selectComponent = false;
                    var valueEditorCollection = this._componentEditors.FirstOrDefault(x => x.Owner == component);
                    var index = this._componentEditors.IndexOf(valueEditorCollection);
                    this._undoService.Do(() => {
                        entity.RemoveComponent(component);
                        this._componentEditors.Remove(valueEditorCollection);

                        if (this.SelectionService.SelectedComponent == component) {
                            this.SelectionService.SelectedComponent = null;
                            selectComponent = true;
                        }
                    }, () => {
                        entity.AddComponent(component);

                        if (this._componentEditors.Count > index) {
                            this._componentEditors.Insert(index, valueEditorCollection);
                        }
                        else {
                            this._componentEditors.Add(valueEditorCollection);
                        }

                        if (selectComponent) {
                            this.SelectionService.SelectedComponent = component;
                        }
                    });
                }
            }

            return Unit.Default;
        }

        private void ResetComponentEditors() {
            if (this.SelectionService.SelectedEntity is IGameScene scene) {
            }
            else if (this.SelectionService.SelectedEntity is IGameEntity entity) {
                var editorCollections = this._valueEditorService.GetComponentEditors(entity, this._removeComponentCommand).ToList();

                lock (this._editorsLock) {
                    foreach (var componentEditor in this._componentEditors) {
                        componentEditor.OwnedValueChanged -= this.EditorCollection_OwnedValueChanged;
                    }

                    foreach (var editorCollection in editorCollections) {
                        editorCollection.OwnedValueChanged += this.EditorCollection_OwnedValueChanged;
                    }

                    this._componentEditors.Reset(editorCollections);
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