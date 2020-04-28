namespace Macabre2D.UI.Library.Controls.ValueEditors {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Services;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ModuleEditor.xaml
    /// </summary>
    public partial class ModuleEditor : NamedValueEditor<BaseModule> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private Type _moduleType;

        public ModuleEditor() {
            this.SelectModuleCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectModuleDialog(this._sceneService.CurrentScene.SavableValue, this._moduleType, out var module)) {
                    this.Value = module;
                }
            });

            this.InitializeComponent();
        }

        public ICommand SelectModuleCommand { get; }

        public override Task Initialize(object value, Type memberType, object owner, string propertName, string title) {
            this._moduleType = memberType;
            return base.Initialize(value, memberType, owner, propertName, title);
        }
    }
}