namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection of entities and their components.
    /// </summary>
    public interface IEntitySelectionService : INotifyPropertyChanged {
        
        /// <summary>
        /// Gets or sets the selected component.
        /// </summary>
        IGameComponent SelectedComponent { get; set; }
        
        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        IGameEntity SelectedEntity { get; set; }
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
            set {
                if (value != this._selectedComponent) {
                    this.RaiseAndSetIfChanged(ref this._selectedComponent, value);
                    
                    if (this._selectedComponent != null) {
                        this.SelectedEntity = this._selectedComponent.Entity;
                    }
                }
            }
        }

        /// <inheritdoc />
        public IGameEntity SelectedEntity {
            get => this._selectedEntity;
            set {
                if (value != this._selectedEntity) {
                    this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
                    this.SelectedComponent = this._selectedEntity?.Components.FirstOrDefault();
                }
            }
        }
    }
}