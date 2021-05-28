namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection of entities and their components.
    /// </summary>
    public interface ISelectionService : INotifyPropertyChanged {
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
    public sealed class SelectionService : ReactiveObject, ISelectionService {
        private EntitySelectionKind _mostRecentlySelectedKind = EntitySelectionKind.None;
        private IEntity _selectedEntity;
        private IUpdateableSystem _selectedSystem;

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

                this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
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
    }
}