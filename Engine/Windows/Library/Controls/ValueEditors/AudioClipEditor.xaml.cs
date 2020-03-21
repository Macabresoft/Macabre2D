namespace Macabre2D.Engine.Windows.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using System.Windows.Input;

    public partial class AudioClipEditor : NamedValueEditor<AudioClip> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public AudioClipEditor() : base() {
            this.SelectAudioClipCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(AudioAsset), true, out var asset)) {
                    this.Value = asset is AudioAsset audioAsset ? audioAsset.AudioClip : null;
                }
            }, true);

            this.InitializeComponent();
        }

        public ICommand SelectAudioClipCommand { get; }
    }
}