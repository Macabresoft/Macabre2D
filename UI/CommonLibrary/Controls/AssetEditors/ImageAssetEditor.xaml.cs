namespace Macabre2D.UI.CommonLibrary.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using Macabre2D.UI.CommonLibrary.Services;
    using System.Collections.Generic;
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

        private readonly IBusyService _busyService = ViewContainer.Resolve<IBusyService>();
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IUndoService _undoService = ViewContainer.Resolve<IUndoService>();
        private SpriteWrapper _selectedSprite;

        public ImageAssetEditor() {
            this.AddSpriteCommand = new RelayCommand(this.AddSprite, () => this.Asset != null);
            this.CloneSpriteCommand = new RelayCommand<SpriteWrapper>(this.CloneSprite, x => x != null);
            this.RemoveSpriteCommand = new RelayCommand<SpriteWrapper>(this.RemoveSprite, x => this.Asset != null && x != null);
            this.MoveDownCommand = new RelayCommand<SpriteWrapper>(this.MoveDown, this.CanMoveDown);
            this.MoveUpCommand = new RelayCommand<SpriteWrapper>(this.MoveUp, this.CanMoveUp);
            this.GenerateSpritesCommand = new RelayCommand(this.GenerateSprites, () => this.Asset != null);
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddSpriteCommand { get; }

        public ImageAsset Asset {
            get { return (ImageAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand CloneSpriteCommand { get; }

        public ICommand GenerateSpritesCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand RemoveSpriteCommand { get; }

        public SpriteWrapper SelectedSprite {
            get {
                return this._selectedSprite;
            }

            set {
                this._selectedSprite = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedSprite)));
            }
        }

        private static void OnAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ImageAssetEditor control && e.NewValue != null) {
                control.SelectedSprite = control.Asset.Sprites.FirstOrDefault();
            }
        }

        private void AddSprite() {
            SpriteWrapper newValue = null;
            var undoCommand = new UndoCommand(() => {
                if (newValue == null) {
                    newValue = this.Asset.AddNewSprite();
                }
                else {
                    this.Asset.AddChild(newValue);
                }

                this.SelectedSprite = newValue;
            },
            () => {
                this.Asset.RemoveChild(newValue);
                this.SelectedSprite = this.Asset.Sprites.FirstOrDefault();
            });

            this._undoService.Do(undoCommand);
        }

        private bool CanMoveDown(SpriteWrapper sprite) {
            return sprite != null && this.Asset.Sprites.IndexOf(sprite) < this.Asset.Sprites.Count - 1;
        }

        private bool CanMoveUp(SpriteWrapper sprite) {
            return sprite != null && this.Asset.Sprites.IndexOf(sprite) > 0;
        }

        private void CloneSprite(SpriteWrapper spriteWrapper) {
            SpriteWrapper newValue = null;
            var undoCommand = new UndoCommand(() => {
                if (newValue == null) {
                    newValue = this.Asset.AddNewSprite();
                    newValue.Size = spriteWrapper.Size;
                    newValue.Location = spriteWrapper.Location;
                    newValue.Name = $"{spriteWrapper.Name} (Clone)";
                }
                else {
                    this.Asset.AddChild(newValue);
                }

                this.SelectedSprite = newValue;
            },
            () => {
                this.Asset.RemoveChild(newValue);
                this.SelectedSprite = this.Asset.Sprites.FirstOrDefault();
            });

            this._undoService.Do(undoCommand);
        }

        private void GenerateSprites() {
            var asset = this.Asset;

            if (asset != null) {
                var result = this._dialogService.ShowGenerateSpritesDialog(asset, out var paramters);

                if (result) {
                    var previousSprites = paramters.ReplaceExistingSprites ? asset.Sprites.ToList() : new List<SpriteWrapper>();

                    var undoCommand = new UndoCommand(() => {
                        this._busyService.PerformAction(() => asset.GenerateSprites(paramters.Columns, paramters.Rows, paramters.ReplaceExistingSprites), true);
                    }, () => {
                        this._busyService.PerformAction(() => {
                            asset.ClearSprites();

                            foreach (var sprite in previousSprites) {
                                asset.AddChild(sprite);
                            }
                        }, true);
                    });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private void MoveDown(SpriteWrapper sprite) {
            var hasChanges = this.Asset.HasChanges;
            var undoCommand = new UndoCommand(() => {
                this.MoveSpriteDown(sprite);
                this.Asset.HasChanges = true;
            }, () => {
                this.MoveSpriteUp(sprite);
                this.Asset.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
        }

        private void MoveSpriteDown(SpriteWrapper sprite) {
            var index = this.Asset.Sprites.IndexOf(sprite);
            this.Asset.Sprites.Move(index, index + 1);
        }

        private void MoveSpriteUp(SpriteWrapper sprite) {
            var index = this.Asset.Sprites.IndexOf(sprite);
            this.Asset.Sprites.Move(index, index - 1);
        }

        private void MoveUp(SpriteWrapper sprite) {
            var hasChanges = this.Asset.HasChanges;
            var undoCommand = new UndoCommand(() => {
                this.MoveSpriteUp(sprite);
                this.Asset.HasChanges = true;
            }, () => {
                this.MoveSpriteDown(sprite);
                this.Asset.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
        }

        private void RemoveSprite(SpriteWrapper spriteWrapper) {
            var originalValue = spriteWrapper;
            var undoCommand = new UndoCommand(() => {
                this.Asset.RemoveChild(originalValue);
                this.SelectedSprite = this.Asset.Sprites.FirstOrDefault();
            }, () => {
                if (this.Asset.AddChild(originalValue)) {
                    this.SelectedSprite = originalValue;
                }
            });

            this._undoService.Do(undoCommand);
        }
    }
}