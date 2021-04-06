namespace Macabresoft.Macabre2D.Editor.Library.ViewModels.Scene {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the scene tree.
    /// </summary>
    public class SceneTreeViewModel : ViewModelBase {
        private readonly ReactiveCommand<IGameEntity, Unit> _addEntityCommand;
        private readonly ReactiveCommand<IGameEntity, Unit> _removeEntityCommand;
        private readonly IProjectService _projectService;
        private readonly ObservableCollection<IGameEntity> _treeRoot = new();
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
        /// <param name="projectService">The scene service.</param>
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public SceneTreeViewModel(IProjectService projectService, ISelectionService selectionService, IUndoService undoService) {
            this._projectService = projectService;
            this.SelectionService = selectionService;
            this._undoService = undoService;
            this.ResetRoot();
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;

            this._addEntityCommand = ReactiveCommand.Create<IGameEntity, Unit>(
                this.AddEntity,
                this.SelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null));

            this._removeEntityCommand = ReactiveCommand.Create<IGameEntity, Unit>(
                this.RemoveEntity,
                this.SelectionService.WhenAny(x => x.SelectedEntity, y => y.Value != null && y.Value.Parent != y.Value));
        }

        /// <summary>
        /// Gets a command to add an entity.
        /// </summary>
        public ICommand AddEntityCommand => this._addEntityCommand;

        /// <summary>
        /// Gets a command to remove an entity.
        /// </summary>
        public ICommand RemoveEntityCommand => this._removeEntityCommand;

        /// <summary>
        /// Gets the root of the scene tree.
        /// </summary>
        public IReadOnlyCollection<IGameEntity> Root => this._treeRoot;

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }

        private Unit AddEntity(IGameEntity parent) {
            var child = new GameEntity {
                Name = "Unnamed Entity"
            };

            var originalHasChanges = this._projectService.HasChanges;
            this._undoService.Do(() => {
                parent.AddChild(child);
                this.SelectionService.SelectedEntity = child;
                this._projectService.HasChanges = true;
            }, () => {
                parent.RemoveChild(child);
                this.SelectionService.SelectedEntity = parent;
                this._projectService.HasChanges = originalHasChanges;
            });

            return Unit.Default;
        }

        private Unit RemoveEntity(IGameEntity entity) {
            var parent = entity.Parent;
            var originalHasChanges = this._projectService.HasChanges;
            this._undoService.Do(() => {
                parent.RemoveChild(entity);
                this.SelectionService.SelectedEntity = null;
                this._projectService.HasChanges = true;
            }, () => {
                parent.AddChild(entity);
                this.SelectionService.SelectedEntity = entity;
                this._projectService.HasChanges = originalHasChanges;
            });

            return Unit.Default;
        }

        private void ResetRoot() {
            this._treeRoot.Clear();

            if (!GameScene.IsNullOrEmpty(this._projectService.CurrentScene)) {
                this._treeRoot.Add(this._projectService.CurrentScene);
            }
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IProjectService.CurrentScene)) {
                this.ResetRoot();
            }
        }
    }
}