namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Windows.Input;

    public partial class PrefabEditor : NamedValueEditor<Prefab> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public PrefabEditor() {
            this.SelectPrefabCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Prefab, AssetType.Prefab, true, out var asset)) {
                    this.Value = asset is PrefabAsset prefabAsset ? prefabAsset.SavableValue : null;
                }
            }, true);

            this.InitializeComponent();
        }

        public ICommand SelectPrefabCommand { get; }
    }
}