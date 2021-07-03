namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Scene {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the scene tree.
    /// </summary>
    public class SceneTreeViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;
        private readonly ISceneService _sceneService;
        private readonly ObservableCollection<IEntity> _treeRoot = new();
        private readonly IUndoService _undoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public SceneTreeViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="entitySelectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public SceneTreeViewModel(IDialogService dialogService, ISceneService sceneService, IEntitySelectionService entitySelectionService, IUndoService undoService) {
            this._dialogService = dialogService;
            this._sceneService = sceneService;
            this.EntitySelectionService = entitySelectionService;
            this._undoService = undoService;
            this.ResetRoot();
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;

            this.AddEntityCommand = ReactiveCommand.CreateFromTask<IEntity>(
                async x => await this.AddEntity(x),
                this.EntitySelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null));

            this.RemoveEntityCommand = ReactiveCommand.Create<IEntity, Unit>(
                this.RemoveEntity,
                this.EntitySelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null && y.Value.Parent != y.Value));
        }

        /// <summary>
        /// Gets a command to add an entity.
        /// </summary>
        public ICommand AddEntityCommand { get; }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntitySelectionService EntitySelectionService { get; }

        /// <summary>
        /// Gets a command to remove an entity.
        /// </summary>
        public ICommand RemoveEntityCommand { get; }

        /// <summary>
        /// Gets the root of the scene tree.
        /// </summary>
        public IReadOnlyCollection<IEntity> Root => this._treeRoot;

        private async Task AddEntity(IEntity parent) {
            var type = await this._dialogService.OpenTypeSelectionDialog(typeof(IEntity), typeof(Scene));
            if (type != null && Activator.CreateInstance(type) is IEntity child) {
                if (type.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute attribute) {
                    child.Name = string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name;
                }
                else {
                    child.Name = type.Name;
                }

                var originalHasChanges = this._sceneService.HasChanges;
                this._undoService.Do(() => {
                    Dispatcher.UIThread.Post(() => {
                        parent.AddChild(child);
                        this.EntitySelectionService.SelectedEntity = child;
                        this._sceneService.HasChanges = true;
                    });
                }, () => {
                    Dispatcher.UIThread.Post(() => {
                        parent.RemoveChild(child);
                        this.EntitySelectionService.SelectedEntity = parent;
                        this._sceneService.HasChanges = originalHasChanges;
                    });
                }, UndoScope.Scene);
            }
        }

        private Unit RemoveEntity(IEntity entity) {
            var parent = entity.Parent;
            var originalHasChanges = this._sceneService.HasChanges;
            this._undoService.Do(() => {
                Dispatcher.UIThread.Post(() => {
                    parent.RemoveChild(entity);
                    this.EntitySelectionService.SelectedEntity = null;
                    this._sceneService.HasChanges = true;
                });
            }, () => {
                Dispatcher.UIThread.Post(() => {
                    parent.AddChild(entity);
                    this.EntitySelectionService.SelectedEntity = entity;
                    this._sceneService.HasChanges = originalHasChanges;
                });
            }, UndoScope.Scene);

            return Unit.Default;
        }

        private void ResetRoot() {
            this._treeRoot.Clear();

            if (!Scene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this._treeRoot.Add(this._sceneService.CurrentScene);
            }
        }

        private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISceneService.CurrentScene)) {
                this.ResetRoot();
            }
        }
    }
}