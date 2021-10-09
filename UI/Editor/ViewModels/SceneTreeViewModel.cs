namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the scene view.
    /// </summary>
    public sealed class SceneTreeBaseViewModel : BaseViewModel {
        private readonly IDialogService _dialogService;
        private readonly ISceneService _sceneService;
        private readonly ObservableCollection<IEntity> _treeRoot = new();
        private readonly IUndoService _undoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeBaseViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public SceneTreeBaseViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeBaseViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="editorService">The editor service.</param>
        /// <param name="entityService">The selection service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public SceneTreeBaseViewModel(
            IDialogService dialogService,
            IEditorService editorService,
            IEntityService entityService,
            ISceneService sceneService,
            IUndoService undoService) {
            this._dialogService = dialogService;
            this.EditorService = editorService;
            this.EntityService = entityService;
            this._sceneService = sceneService;
            this._undoService = undoService;
            this.ResetRoot();
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;

            this.AddEntityCommand = ReactiveCommand.CreateFromTask<Type>(
                async x => await this.AddEntity(x),
                this.EntityService.WhenAny(x => x.Selected, y => y.Value != null));

            this.RemoveEntityCommand = ReactiveCommand.Create<IEntity>(
                this.RemoveEntity,
                this.EntityService.WhenAny(x => x.Selected, y => y.Value != null && y.Value.Parent != y.Value));

            this.RenameCommand = ReactiveCommand.Create<string>(
                this.RenameEntity,
                this.EntityService.WhenAny(x => x.Selected, y => y.Value != null));
        }

        /// <summary>
        /// Gets a command to add an entity.
        /// </summary>
        public ICommand AddEntityCommand { get; }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntityService EntityService { get; }

        /// <summary>
        /// Gets a command to remove an entity.
        /// </summary>
        public ICommand RemoveEntityCommand { get; }

        /// <summary>
        /// Gets a command for renaming an entity.
        /// </summary>
        public ICommand RenameCommand { get; }

        /// <summary>
        /// Gets the root of the scene tree.
        /// </summary>
        public IReadOnlyCollection<IEntity> Root => this._treeRoot;

        /// <summary>
        /// Moves the source entity to be a child of the target entity.
        /// </summary>
        /// <param name="sourceEntity">The source entity.</param>
        /// <param name="targetEntity">The target entity.</param>
        public void MoveEntity(IEntity sourceEntity, IEntity targetEntity) {
            if (CanMoveEntity(sourceEntity, targetEntity)) {
                var originalParent = sourceEntity.Parent;
                this._undoService.Do(() => { targetEntity.AddChild(sourceEntity); }, () => { originalParent.AddChild(sourceEntity); });
            }
        }

        private async Task AddEntity(Type type) {
            type ??= await this._dialogService.OpenTypeSelectionDialog(this.EntityService.AvailableTypes);

            if (type != null && Activator.CreateInstance(type) is IEntity child) {
                if (type.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute attribute) {
                    child.Name = string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name;
                }
                else {
                    child.Name = type.Name;
                }

                var parent = this.EntityService.Selected;

                this._undoService.Do(() => {
                    Dispatcher.UIThread.Post(() => {
                        parent.AddChild(child);
                        this.EntityService.Selected = child;
                    });
                }, () => {
                    Dispatcher.UIThread.Post(() => {
                        parent.RemoveChild(child);
                        this.EntityService.Selected = parent;
                    });
                });
            }
        }

        private static bool CanMoveEntity(IEntity sourceEntity, IEntity targetEntity) {
            return sourceEntity != null &&
                   targetEntity != null &&
                   sourceEntity != targetEntity &&
                   !targetEntity.IsDescendentOf(sourceEntity);
        }

        private void RemoveEntity(IEntity entity) {
            var parent = entity.Parent;
            this._undoService.Do(() => {
                Dispatcher.UIThread.Post(() => {
                    parent.RemoveChild(entity);
                    this.EntityService.Selected = null;
                });
            }, () => {
                Dispatcher.UIThread.Post(() => {
                    parent.AddChild(entity);
                    this.EntityService.Selected = entity;
                });
            });
        }

        private void RenameEntity(string updatedName) {
            if (this.EntityService.Selected is IEntity entity && entity.Name != updatedName) {
                var originalName = entity.Name;
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { entity.Name = updatedName; }); }, () => { Dispatcher.UIThread.Post(() => { entity.Name = originalName; }); });
            }
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