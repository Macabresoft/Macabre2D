namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public partial class ModuleEditor : NamedValueEditor<BaseModule> {
        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly ISceneService _sceneService = ViewContainer.Resolve<ISceneService>();
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