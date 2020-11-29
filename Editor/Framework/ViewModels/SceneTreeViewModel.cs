namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Windows.Input;
    using DynamicData.Binding;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the scene tree.
    /// </summary>
    public class SceneTreeViewModel : ViewModelBase {
        private readonly ISceneService _sceneService;
        private readonly ObservableCollection<IGameEntity> _treeRoot = new();
        private readonly ReactiveCommand<IGameEntity, Unit> _addEntityCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public SceneTreeViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        [InjectionConstructor]
        public SceneTreeViewModel(ISceneService sceneService, IEntitySelectionService selectionService) {
            this._sceneService = sceneService;
            this.SelectionService = selectionService;
            this.ResetRoot();
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;

            this._addEntityCommand = ReactiveCommand.Create<IGameEntity, Unit>(
                AddEntity,
                this.SelectionService.WhenAny(x => x.SelectedEntity, y => y != null));
        }

        /// <summary>
        /// Gets a command to add an entity.
        /// </summary>
        public ICommand AddEntityCommand => this._addEntityCommand;

        /// <summary>
        /// Gets the root of the scene tree.
        /// </summary>
        public IReadOnlyCollection<IGameEntity> Root => this._treeRoot;

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntitySelectionService SelectionService { get; }

        private static Unit AddEntity(IGameEntity parent) {
            var child = parent.AddChild();
            child.Name = "Unnamed Entity";
            return Unit.Default;
        }

        private void ResetRoot() {
            this._treeRoot.Clear();

            if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this._treeRoot.Add(this._sceneService.CurrentScene);
            }
        }

        private void SceneService_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISceneService.CurrentScene)) {
                this.ResetRoot();
            }
        }
    }
}