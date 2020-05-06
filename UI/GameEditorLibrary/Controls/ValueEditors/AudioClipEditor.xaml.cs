namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System.Windows.Input;

    public partial class AudioClipEditor : NamedValueEditor<AudioClip> {
        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public AudioClipEditor() : base() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(AudioAsset), true, out var asset)) {
                    this.Value = asset is AudioAsset audioAsset ? audioAsset.AudioClip : null;
                }
            }, true);

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

            this.InitializeComponent();
        }

        public ICommand ClearCommand { get; }

        public ICommand SelectCommand { get; }
    }
}