﻿namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Models.FrameworkWrappers;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteEditor : NamedValueEditor<Sprite> {
        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private string _absolutePathToImage;
        private SpriteWrapper _spriteWrapper;

        public SpriteEditor() : base() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectSpriteDialog(this.SpriteWrapper, out var spriteWrapper)) {
                    this.SpriteWrapper = spriteWrapper;
                }
            }, true);

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

            this.Loaded += this.SpriteEditor_Loaded;
            this.InitializeComponent();
        }

        public ICommand ClearCommand { get; }

        public ICommand SelectCommand { get; }

        public SpriteWrapper SpriteWrapper {
            get {
                return this._spriteWrapper;
            }

            private set {
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
                this._spriteWrapper = spriteWrappers.FirstOrDefault(x => x.Sprite?.Id == this.Value.Id);
                this.RaisePropertyChanged(nameof(this.SpriteWrapper));
            }

            this.Loaded -= this.SpriteEditor_Loaded;
        }
    }
}