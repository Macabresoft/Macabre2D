namespace Macabre2D.UI.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class ImageAssetEditor : UserControl, INotifyPropertyChanged {

        public static DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(ImageAsset),
            typeof(ImageAssetEditor),
            new PropertyMetadata(null, new PropertyChangedCallback(OnAssetChanged)));

        private readonly RelayCommand _removeSpriteCommand;
        private readonly IUndoService _undoService;
        private SpriteWrapper _selectedSprite;

        public ImageAssetEditor() {
            this._undoService = ViewContainer.Resolve<IUndoService>();
            this.AddSpriteCommand = new RelayCommand(this.AddSprite, () => this.Asset != null);
            this._removeSpriteCommand = new RelayCommand(this.RemoveSprite, () => this.Asset != null && this.SelectedSprite != null);
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddSpriteCommand { get; }

        public ImageAsset Asset {
            get { return (ImageAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand RemoveSpriteCommand {
            get {
                return this._removeSpriteCommand;
            }
        }

        public SpriteWrapper SelectedSprite {
            get {
                return this._selectedSprite;
            }

            set {
                this._selectedSprite = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedSprite)));
                this._removeSpriteCommand.RaiseCanExecuteChanged();
            }
        }

        private static void OnAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ImageAssetEditor control && e.NewValue != null) {
                control.SelectedSprite = control.Asset.Children.FirstOrDefault();
            }
        }

        private void AddSprite() {
            SpriteWrapper newValue = null;
            var undoCommand = new UndoCommand(() => {
                if (newValue == null) {
                    newValue = this.Asset.AddNewSprite();
                }
                else if (this.Asset.AddChild(newValue)) {
                    this.SelectedSprite = newValue;
                }
            },
            () => {
                this.Asset.RemoveChild(newValue);
                this.SelectedSprite = this.Asset.Children.FirstOrDefault();
            });

            this._undoService.Do(undoCommand);
        }

        private void RemoveSprite() {
            var originalValue = this.SelectedSprite;
            var undoCommand = new UndoCommand(() => {
                this.Asset.RemoveChild(originalValue);
                this.SelectedSprite = this.Asset.Children.FirstOrDefault();
            }, () => {
                if (this.Asset.AddChild(originalValue)) {
                    this.SelectedSprite = originalValue;
                }
            });

            this._undoService.Do(undoCommand);
        }
    }
}