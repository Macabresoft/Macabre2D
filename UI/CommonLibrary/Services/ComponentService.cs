namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System.ComponentModel;

    public interface IComponentService : INotifyPropertyChanged {
        BaseComponent SelectedItem { get; set; }

        void ResetSelectedItemBoundingArea();
    }

    public sealed class ComponentService : NotifyPropertyChanged, IComponentService {
        private readonly ISceneService _sceneService;
        private BaseComponent _selectedItem;

        public ComponentService(ISceneService sceneService) {
            this._sceneService = sceneService;
        }

        public BaseComponent SelectedItem {
            get {
                return this._selectedItem;
            }

            set {
                var oldValue = this._selectedItem;
                if (this.Set(ref this._selectedItem, value)) {
                    this.ResetSelectedItemBoundingArea();

                    if (this._selectedItem != null) {
                        this._selectedItem.PropertyChanged += this.SelectedItem_PropertyChanged;
                    }

                    if (oldValue != null) {
                        oldValue.PropertyChanged -= this.SelectedItem_PropertyChanged;
                    }
                }
            }
        }

        public void ResetSelectedItemBoundingArea() {
            // This is a bit weird, but it forces the Bounding Area to be reset in case the pixels
            // per unit has changed.
            if (this._selectedItem != null) {
                var originalPosition = this._selectedItem.LocalPosition;
                this._selectedItem.LocalPosition = Vector2.Zero;
                this._selectedItem.LocalPosition = originalPosition;
            }
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this._sceneService.CurrentScene.HasChanges = true;
        }
    }
}