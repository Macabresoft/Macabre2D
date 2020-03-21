namespace Macabre2D.Engine.Windows.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using Macabre2D.Framework;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteAnimationEditor : NamedValueEditor<SpriteAnimation> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(SpriteAnimationAsset),
            typeof(SpriteAnimationEditor),
            new PropertyMetadata());

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public SpriteAnimationEditor() : base() {
            this.SelectSpriteAnimationCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(SpriteAnimationAsset), true, out var asset)) {
                    this.Asset = asset as SpriteAnimationAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.Loaded += this.SpriteAnimationEditor_Loaded;
            this.InitializeComponent();
        }

        public SpriteAnimationAsset Asset {
            get { return (SpriteAnimationAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand SelectSpriteAnimationCommand { get; }

        private void SpriteAnimationEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var spriteAnimationAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<SpriteAnimationAsset>();
                this.Asset = spriteAnimationAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= this.SpriteAnimationEditor_Loaded;
        }
    }
}