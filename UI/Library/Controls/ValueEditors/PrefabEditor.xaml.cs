namespace Macabre2D.UI.Library.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using System.Windows.Input;

    public partial class PrefabEditor : NamedValueEditor<Prefab> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public PrefabEditor() : base() {
            this.SelectPrefabCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(PrefabAsset), true, out var asset)) {
                    this.Value = asset is PrefabAsset prefabAsset ? prefabAsset.SavableValue : null;
                }
            }, true);

            this.InitializeComponent();
        }

        public ICommand SelectPrefabCommand { get; }
    }
}