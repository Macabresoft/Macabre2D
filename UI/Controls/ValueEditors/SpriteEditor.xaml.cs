namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteEditor : NamedValueEditor<Sprite>, ISeparatedValueEditor {

        public static readonly DependencyProperty ShowBottomSeparatorProperty = DependencyProperty.Register(
            nameof(ShowBottomSeparator),
            typeof(bool),
            typeof(SpriteEditor),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTopSeparatorProperty = DependencyProperty.Register(
            nameof(ShowTopSeparator),
            typeof(bool),
            typeof(SpriteEditor),
            new PropertyMetadata(true));

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private string _absolutePathToImage;
        private SpriteWrapper _spriteWrapper;

        public SpriteEditor() {
            this.SelectSpriteCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectSpriteDialog(out var spriteWrapper)) {
                    this.SpriteWrapper = spriteWrapper;
                }
            }, true);

            this.Loaded += this.SpriteEditor_Loaded;
            this.InitializeComponent();
        }

        public ICommand SelectSpriteCommand { get; }

        public bool ShowBottomSeparator {
            get { return (bool)this.GetValue(ShowBottomSeparatorProperty); }
            set { this.SetValue(ShowBottomSeparatorProperty, value); }
        }

        public bool ShowTopSeparator {
            get { return (bool)this.GetValue(ShowTopSeparatorProperty); }
            set { this.SetValue(ShowTopSeparatorProperty, value); }
        }

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
                var spriteWrappers = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites);
                this.SpriteWrapper = spriteWrappers.FirstOrDefault(x => x.Sprite?.Id == this.Value.Id);
            }

            this.Loaded -= this.SpriteEditor_Loaded;
        }
    }
}