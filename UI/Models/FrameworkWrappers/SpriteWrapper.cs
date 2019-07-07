namespace Macabre2D.UI.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    public sealed class SpriteWrapper : Asset {
        private ImageAsset _imageAsset;
        private Sprite _sprite;

        public SpriteWrapper(Sprite sprite, ImageAsset imageAsset) {
            this.Sprite = sprite;
            this.ImageAsset = imageAsset;
        }

        public int Height {
            get {
                return this.Size.Y;
            }
        }

        public ImageAsset ImageAsset {
            get {
                return this._imageAsset;
            }

            set {
                this.Set(ref this._imageAsset, value);
            }
        }

        public Point Location {
            get {
                if (this.Sprite != null) {
                    return this.Sprite.Location;
                }

                return Point.Zero;
            }

            set {
                this.Sprite.Location = new Point(
                    Math.Min(value.X, this.ImageAsset.Width - this.Size.X),
                    Math.Min(value.Y, this.ImageAsset.Height - this.Size.Y));
                this.RaisePropertyChanged();
            }
        }

        public Point Size {
            get {
                if (this.Sprite != null) {
                    return this.Sprite.Size;
                }

                return Point.Zero;
            }

            set {
                this.Sprite.Size = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.Height));
                this.RaisePropertyChanged(nameof(this.Width));
            }
        }

        [DataMember]
        public Sprite Sprite {
            get {
                return this._sprite;
            }

            private set {
                if (this.Set(ref this._sprite, value)) {
                    this._sprite.Name = this.Name;
                }
            }
        }

        public override AssetType Type {
            get {
                return AssetType.Sprite;
            }
        }

        public int Width {
            get {
                return this.Size.X;
            }
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.Sprite.Id);
        }

        public override string GetContentPath() {
            return this.ImageAsset.GetContentPath();
        }

        public override string GetPath() {
            return this.ImageAsset?.GetPath();
        }

        private void SpriteWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Name)) {
                this.Sprite.Name = this.Name;
            }
            else if (e.PropertyName == nameof(this.Sprite)) {
                this.RaisePropertyChanged(nameof(this.Name));
                this.RaisePropertyChanged(nameof(this.Location));
                this.RaisePropertyChanged(nameof(this.Size));
            }
        }
    }
}