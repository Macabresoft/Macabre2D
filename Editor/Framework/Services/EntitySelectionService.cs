namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// The kind of selection.
    /// </summary>
    public enum EntitySelectionKind {
        None,
        Entity,
        Scene
    }

    /// <summary>
    /// An interface for a service which handles the selection of entities and their components.
    /// </summary>
    public interface ISelectionService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the most recently selected item
        /// </summary>
        EntitySelectionKind MostRecentlySelectedKind { get; }

        /// <summary>
        /// Gets or sets the selected component.
        /// </summary>
        IGameComponent SelectedComponent { get; set; }

        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        IGameEntity SelectedEntity { get; set; }

        /// <summary>
        /// Gets or sets the selected system.
        /// </summary>
        IGameSystem SelectedSystem { get; set; }
    }

    /// <summary>
    /// A service which handles the selection of entities and their components.
    /// </summary>
    public sealed class SelectionService : ReactiveObject, ISelectionService {
        private EntitySelectionKind _mostRecentlySelectedKind = EntitySelectionKind.None;
        private IGameComponent _selectedComponent;
        private IGameEntity _selectedEntity;
        private IGameSystem _selectedSystem;

        /// <inheritdoc />
        public EntitySelectionKind MostRecentlySelectedKind {
            get => this._mostRecentlySelectedKind;
            private set => this.RaiseAndSetIfChanged(ref this._mostRecentlySelectedKind, value);
        }

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
                if (value is IGameScene scene) {
                    if (value != this._selectedEntity) {
                        this.SelectedSystem = scene.Systems.FirstOrDefault();
                    }

                    this.MostRecentlySelectedKind = EntitySelectionKind.Scene;
                }
                else if (value != null) {
                    if (value != this._selectedEntity) {
                        this.SelectedComponent = value.Components.FirstOrDefault();
                    }

                    this.MostRecentlySelectedKind = EntitySelectionKind.Entity;
                }
                else {
                    this.MostRecentlySelectedKind = EntitySelectionKind.None;
                    this.SelectedComponent = null;
                    this.SelectedSystem = null;
                }

                this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
            }
        }

        /// <inheritdoc />
        public IGameSystem SelectedSystem {
            get => this._selectedSystem;
            set {
                if (value != this._selectedSystem) {
                    this.RaiseAndSetIfChanged(ref this._selectedSystem, value);
                }

                if (this._selectedEntity != null) {
                    this._mostRecentlySelectedKind = this._selectedEntity is IGameScene ? EntitySelectionKind.Scene : EntitySelectionKind.Entity;
                }
                else if (this._mostRecentlySelectedKind == EntitySelectionKind.Entity || this._mostRecentlySelectedKind == EntitySelectionKind.Scene) {
                    this.MostRecentlySelectedKind = EntitySelectionKind.None;
                }
            }
        }
    }
}