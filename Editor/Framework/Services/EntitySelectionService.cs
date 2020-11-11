namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection of entities and their components.
    /// </summary>
    public interface IEntitySelectionService : INotifyPropertyChanged {
        
        /// <summary>
        /// Gets the selected component.
        /// </summary>
        IGameComponent SelectedComponent { get; }
        
        /// <summary>
        /// Gets the selected entity.
        /// </summary>
        IGameEntity SelectedEntity { get; }

        /// <summary>
        /// Selects the provided <see cref="IGameEntity"/> and alerts all other services of the change.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Select(IGameEntity entity);

        /// <summary>
        /// Selects the provided <see cref="IGameComponent"/> and alerts all other services of the change.
        /// </summary>
        /// <param name="component"></param>
        void Select(IGameComponent component);
    }
    
    /// <summary>
    /// A service which handles the selection of entities and their components.
    /// </summary>
    public sealed class EntitySelectionService : ReactiveObject, IEntitySelectionService {

        private IGameComponent _selectedComponent;
        private IGameEntity _selectedEntity;


        /// <inheritdoc />
        public IGameComponent SelectedComponent {
            get => this._selectedComponent;
            private set {
                this.RaiseAndSetIfChanged(ref this._selectedComponent, value);
            }
        }

        /// <inheritdoc />
        public IGameEntity SelectedEntity {
            get => this._selectedEntity;
            private set {
                this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
            }
        }
        
        /// <inheritdoc />
        public void Select(IGameEntity entity) {
            this.SelectedEntity = entity;
        }

        /// <inheritdoc />
        public void Select(IGameComponent component) {
            this.SelectedComponent = component;
        }
    }
}