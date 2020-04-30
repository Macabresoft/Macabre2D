namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using Microsoft.Xna.Framework;
    using System;
    using System.ComponentModel;

    public interface IComponentService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        ComponentWrapper SelectedItem { get; set; }

        void ResetSelectedItemBoundingArea();

        void SelectComponent(BaseComponent component);
    }

    public sealed class ComponentService : NotifyPropertyChanged, IComponentService {
        private readonly ISceneService _sceneService;

        private ComponentWrapper _selectedItem;

        public ComponentService(ISceneService sceneService) {
            this._sceneService = sceneService;
        }

        public event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        public ComponentWrapper SelectedItem {
            get {
                return this._selectedItem;
            }

            set {
                var oldValue = this._selectedItem;
                if (this.Set(ref this._selectedItem, value)) {
                    this.SelectionChanged.SafeInvoke(this, new ValueChangedEventArgs<ComponentWrapper>(oldValue, value));
                    this.ResetSelectedItemBoundingArea();
                }
            }
        }

        public void ResetSelectedItemBoundingArea() {
            // This is a bit weird, but it forces the Bounding Area to be reset in case the pixels
            // per unit has changed.
            if (this._selectedItem?.Component != null) {
                var originalPosition = this._selectedItem.Component.LocalPosition;
                this._selectedItem.Component.LocalPosition = Vector2.Zero;
                this._selectedItem.Component.LocalPosition = originalPosition;
            }
        }

        public void SelectComponent(BaseComponent component) {
            if (component == null) {
                this.SelectedItem = null;
            }
            else if (this._sceneService.CurrentScene != null) {
                ComponentWrapper result = null;
                foreach (var child in this._sceneService.CurrentScene.Children) {
                    if (child.Id == component.Id) {
                        result = child;
                    }
                    else {
                        result = this.FindWrapperInChildren(child, component);
                    }

                    if (result != null) {
                        this.SelectedItem = result;
                        break;
                    }
                }
            }
        }

        private ComponentWrapper FindWrapperInChildren(ComponentWrapper parent, BaseComponent component) {
            ComponentWrapper result = null;
            foreach (var child in parent.Children) {
                if (child.Id == component.Id) {
                    result = child;
                }
                else {
                    result = this.FindWrapperInChildren(child, component);
                }

                if (result != null) {
                    break;
                }
            }

            return result;
        }
    }
}