namespace Macabre2D.UI.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public sealed class SpriteWrapper : NotifyPropertyChanged {

        [DataMember]
        private ImageAsset _imageAsset;

        [DataMember]
        private Sprite _sprite;

        public SpriteWrapper(Sprite sprite, ImageAsset imageAsset) {
            this.Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
            this.ImageAsset = imageAsset ?? throw new ArgumentNullException(nameof(imageAsset));
        }

        public int Height {
            get {
                return this.Size.Y;
            }
        }

        public Guid Id {
            get {
                return this.Sprite != null ? this.Sprite.Id : Guid.Empty;
            }
        }

        public ImageAsset ImageAsset {
            get {
                return this._imageAsset;
            }

            private set {
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
                if (this.ImageAsset.Width != 0 && this.ImageAsset.Height != 0) {
                    this.Sprite.Location = new Point(
                        Math.Min(value.X, this.ImageAsset.Width - this.Size.X),
                        Math.Min(value.Y, this.ImageAsset.Height - this.Size.Y));
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Name {
            get {
                return this.Sprite?.Name;
            }

            set {
                this.Sprite.Name = value;
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

        public Sprite Sprite {
            get {
                return this._sprite;
            }

            private set {
                this.Set(ref this._sprite, value);
            }
        }

        public int Width {
            get {
                return this.Size.X;
            }
        }

        private void SpriteWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Sprite)) {
                this.RaisePropertyChanged(nameof(this.Name));
                this.RaisePropertyChanged(nameof(this.Location));
                this.RaisePropertyChanged(nameof(this.Size));
            }
        }
    }
}