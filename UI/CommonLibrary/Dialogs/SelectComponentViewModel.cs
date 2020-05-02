namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Models.Validation;
    using System;

    public class SelectComponentViewModel : OKCancelDialogViewModel {
        private BaseComponent _selectedComponent;

        public SelectComponentViewModel(SceneAsset sceneAsset, Type requestedType) {
            this.Scene = sceneAsset;
            this.RequestedType = requestedType;
        }

        public Type RequestedType { get; }

        public SceneAsset Scene { get; }

        [RequiredValidation]
        public BaseComponent SelectedComponent {
            get {
                return this._selectedComponent;
            }

            set {
                if (this.Set(ref this._selectedComponent, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        protected override bool CanExecuteOKCommand() {
            return this.SelectedComponent != null && this.RequestedType.IsAssignableFrom(this.SelectedComponent.GetType());
        }
    }
}