using Macabre2D.UI.CommonLibrary.Models;
using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
using Macabre2D.UI.CommonLibrary.Models.Validation;
using System;

namespace Macabre2D.UI.CommonLibrary.Dialogs {

    public class SelectComponentViewModel : OKCancelDialogViewModel {
        private ComponentWrapper _selectedComponent;

        public SelectComponentViewModel(SceneAsset sceneAsset, Type requestedType) {
            this.Scene = sceneAsset;
            this.RequestedType = requestedType;
        }

        public Type RequestedType { get; }

        public SceneAsset Scene { get; }

        [RequiredValidation]
        public ComponentWrapper SelectedComponent {
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
            return this.SelectedComponent?.Component != null && RequestedType.IsAssignableFrom(this.SelectedComponent.Component?.GetType());
        }
    }
}