namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public partial class ComponentEditor : NamedValueEditor<BaseComponent> {
        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly ISceneService _sceneService = ViewContainer.Resolve<ISceneService>();
        private Type _componentType;

        public ComponentEditor() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectComponentDialog(this._sceneService.CurrentScene, this._componentType, out var component)) {
                    this.Value = component;
                }
            });

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

            this.InitializeComponent();
        }

        public ICommand ClearCommand { get; }

        public ICommand SelectCommand { get; }

        public override Task Initialize(object value, Type memberType, object owner, string propertName, string title) {
            this._componentType = memberType;
            return base.Initialize(value, memberType, owner, propertName, title);
        }
    }
}