namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteEditor : NamedValueEditor<Sprite> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private string _absolutePathToImage;
        private SpriteWrapper _spriteWrapper;

        public SpriteEditor() : base() {
            this.SelectSpriteCommand = new RelayCommand(() => {
                var asset = this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Image | AssetType.Sprite, AssetType.Sprite);
                if (asset is SpriteWrapper spriteWrapper) {
                    this.SpriteWrapper = spriteWrapper;
                }
            }, true);

            this.Loaded += this.SpriteEditor_Loaded;
            this.InitializeComponent();
        }

        public ICommand SelectSpriteCommand { get; }

        public SpriteWrapper SpriteWrapper {
            get {
                return this._spriteWrapper;
            }

            set {
                this._spriteWrapper = value;
                this.Value = this._spriteWrapper?.Sprite;
                this.RaisePropertyChanged(nameof(this.SpriteWrapper));
            }
        }

        internal string AbsolutePathToImage {
            get {
                return this._absolutePathToImage;
            }
            set {
                if (value != this._absolutePathToImage) {
                    this._absolutePathToImage = value;
                    this.RaisePropertyChanged(this.AbsolutePathToImage);
                }
            }
        }

        protected override void OnValueChanged(Sprite newValue, Sprite oldValue, DependencyObject d) {
            if (d is SpriteEditor editor) {
                editor.RaisePropertyChanged(nameof(this.SpriteWrapper));
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void SpriteEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.SpriteWrapper == null || this.SpriteWrapper.Sprite?.Id != this.Value.Id)) {
                var spriteWrappers = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<SpriteWrapper>();
                this.SpriteWrapper = spriteWrappers.FirstOrDefault(x => x.Sprite.Id == this.Value.Id);
            }

            this.Loaded -= this.SpriteEditor_Loaded;
        }
    }
}