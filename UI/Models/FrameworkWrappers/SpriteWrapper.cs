namespace Macabre2D.UI.Models.FrameworkWrappers
{
    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    public sealed class SpriteWrapper : Asset
    {
        private ImageAsset _imageAsset;
        private Sprite _sprite;

        public SpriteWrapper(Sprite sprite, ImageAsset imageAsset) : this()
        {
            this.Sprite = sprite;
            this.ImageAsset = imageAsset;
        }

        internal SpriteWrapper()
        {
            this.PropertyChanged += this.SpriteWrapper_PropertyChanged;
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
                var oldImageAsset = this._imageAsset;
                if (this.Set(ref this._imageAsset, value))
                {
                    if (this._imageAsset != null)
                    {
                        this._imageAsset.PropertyChanged += this.ImageAsset_PropertyChanged;
                    }

                    if (oldImageAsset != null)
                    {
                        oldImageAsset.PropertyChanged -= this.ImageAsset_PropertyChanged;
                    }
                }
            }
        }

        public Point Location {
            get {
                if (this.Sprite != null)
                {
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
                if (this.Sprite != null)
                {
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
                if (this.Set(ref this._sprite, value))
                {
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

        public override void Delete()
        {
            this.RemoveIdentifiableContentFromScenes(this.Sprite.Id);
        }

        public override string GetContentPath()
        {
            return this.ImageAsset.GetContentPath();
        }

        public override string GetPath()
        {
            return this.ImageAsset?.GetPath();
        }

        internal override void MoveAsset(string originalPath, string newPath)
        {
            return; // A sprite being renamed doesn't require an asset rename, as it is stored inside of an ImageAsset. This is a weird hack, sorry.
        }

        private void ImageAsset_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Parent.Name) || e.PropertyName == nameof(this.Parent.Parent))
            {
                this.Sprite.ContentPath = Path.ChangeExtension(this.GetContentPath(), null);
            }
        }

        private void SpriteWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Name))
            {
                this.Sprite.Name = this.Name;
            }
            else if (e.PropertyName == nameof(this.Sprite))
            {
                this.RaisePropertyChanged(nameof(this.Name));
                this.RaisePropertyChanged(nameof(this.Location));
                this.RaisePropertyChanged(nameof(this.Size));
            }
        }
    }
}