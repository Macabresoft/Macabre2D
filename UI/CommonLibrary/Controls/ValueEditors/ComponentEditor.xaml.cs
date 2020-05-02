namespace Macabre2D.UI.CommonLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Services;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public partial class ComponentEditor : NamedValueEditor<BaseComponent> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private Type _componentType;

        public ComponentEditor() {
            this.SelectComponentCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectComponentDialog(this._sceneService.CurrentScene, this._componentType, out var component)) {
                    this.Value = component;
                }
            });

            this.InitializeComponent();
        }

        public ICommand SelectComponentCommand { get; }

        public override Task Initialize(object value, Type memberType, object owner, string propertName, string title) {
            this._componentType = memberType;
            return base.Initialize(value, memberType, owner, propertName, title);
        }
    }
}