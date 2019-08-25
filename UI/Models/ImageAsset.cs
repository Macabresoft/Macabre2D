namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Media.Imaging;

    public sealed class ImageAsset : MetadataAsset {

        [DataMember]
        private readonly ObservableCollection<SpriteWrapper> _children = new ObservableCollection<SpriteWrapper>();

        [DataMember]
        private int _height;

        [DataMember]
        private int _width;

        public ImageAsset(string name) : base(name) {
            this._children.CollectionChanged += this.Children_CollectionChanged;
        }

        public ImageAsset() : this(string.Empty) {
        }

        public int Height {
            get {
                return this._height;
            }

            set {
                this.Set(ref this._height, value);
            }
        }

        public IReadOnlyCollection<SpriteWrapper> Sprites {
            get {
                return this._children;
            }
        }

        public override AssetType Type {
            get {
                return AssetType.Image;
            }
        }

        public int Width {
            get {
                return this._width;
            }

            set {
                this.Set(ref this._width, value);
            }
        }

        public bool AddChild(SpriteWrapper wrapper) {
            var result = false;
            if (!this.Sprites.Contains(wrapper) && wrapper.ImageAsset == this) {
                result = true;
                wrapper.PropertyChanged += this.Child_PropertyChanged;
                wrapper.Sprite.AssetId = this.Id;
                this._children.Add(wrapper);
                this.RaisePropertyChanged(nameof(this.Sprites));
            }

            return result;
        }

        public SpriteWrapper AddNewSprite() {
            var contentPath = this.GetContentPath();
            var sprite = new Sprite {
                AssetId = this.Id,
                Size = new Point(this.Width, this.Height)
            };

            var wrapper = new SpriteWrapper(sprite, this);
            var name = Path.GetFileNameWithoutExtension(this.Name);
            var counter = 0;
            var tempName = $"{name}-{counter.ToString("000")}";
            while (this.Sprites.Any(x => x.Name == tempName)) {
                counter++;
                tempName = $"{name}-{counter.ToString("000")}";
            }

            wrapper.Name = tempName;
            this.AddChild(wrapper);
            return wrapper;
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string contentPath) {
            var path = Path.Combine(contentPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:TextureImporter");
            contentStringBuilder.AppendLine(@"/processor:TextureProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:ColorKeyColor=255,0,255,255");
            contentStringBuilder.AppendLine(@"/processorParam:ColorKeyEnabled=True");
            contentStringBuilder.AppendLine(@"/processorParam:GenerateMipmaps=False");
            contentStringBuilder.AppendLine(@"/processorParam:PremultiplyAlpha=True");
            contentStringBuilder.AppendLine(@"/processorParam:ResizeToPowerOfTwo=False");
            contentStringBuilder.AppendLine(@"/processorParam:MakeSquare=False");
            contentStringBuilder.AppendLine(@"/processorParam:TextureFormat=Color");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Delete() {
            foreach (var child in this._children) {
                if (child.Sprite != null) {
                    this.RemoveIdentifiableContentFromScenes(child.Sprite.AssetId);
                }
            }

            this._children.Clear();
            base.Delete();
        }

        public override void Refresh(AssetManager assetManager) {
            using (var imageStream = File.OpenRead(this.GetPath())) {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                this.Width = decoder.Frames[0].PixelWidth;
                this.Height = decoder.Frames[0].PixelHeight;
            }

            assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
            base.Refresh(assetManager);
        }

        public bool RemoveChild(SpriteWrapper spriteWrapper) {
            var result = false;

            if (this._children.Remove(spriteWrapper)) {
                result = true;
                spriteWrapper.PropertyChanged -= this.Child_PropertyChanged;
                this.HandleSpriteWrapperRemoved(spriteWrapper);
            }

            return result;
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Sprites));
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var child in e.NewItems.Cast<SpriteWrapper>()) {
                    child.PropertyChanged += this.Child_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var child in e.OldItems.Cast<SpriteWrapper>()) {
                    child.PropertyChanged -= this.Child_PropertyChanged;
                }
            }
        }

        private void HandleSpriteWrapperRemoved(SpriteWrapper spriteWrapper) {
            if (spriteWrapper.Sprite != null) {
                this.RemoveIdentifiableContentFromScenes(spriteWrapper.Sprite.Id);
            }
        }
    }
}