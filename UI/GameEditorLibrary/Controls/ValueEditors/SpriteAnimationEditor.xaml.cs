namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteAnimationEditor : NamedValueEditor<SpriteAnimation> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(SpriteAnimationAsset),
            typeof(SpriteAnimationEditor),
            new PropertyMetadata());

        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public SpriteAnimationEditor() : base() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(SpriteAnimationAsset), true, out var asset)) {
                    this.Asset = asset as SpriteAnimationAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

            this.Loaded += this.SpriteAnimationEditor_Loaded;
            this.InitializeComponent();
        }

        public SpriteAnimationAsset Asset {
            get { return (SpriteAnimationAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand ClearCommand { get; }

        public ICommand SelectCommand { get; }

        private void SpriteAnimationEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var spriteAnimationAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<SpriteAnimationAsset>();
                this.Asset = spriteAnimationAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= this.SpriteAnimationEditor_Loaded;
        }
    }
}