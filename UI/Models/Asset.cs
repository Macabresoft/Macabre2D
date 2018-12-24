namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    [Flags]
    public enum AssetType {
        Folder = 0,

        File = 1 << 0,

        Scene = 1 << 1,

        Image = 1 << 2,

        Audio = 1 << 3,

        Sprite = 1 << 4,

        SpriteAnimation = 1 << 5,

        Font = 1 << 6,

        All = ~Folder
    }

    [DataContract]
    public class Asset : NotifyPropertyChanged {

        [DataMember]
        private string _name;

        private FolderAsset _parent;

        public Asset(string name) {
            this._name = name;
        }

        internal Asset() {
        }

        [DataMember]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

        public FolderAsset Parent {
            get {
                return this._parent;
            }

            set {
                var originalParent = this._parent as FolderAsset;
                if (this.Set(ref this._parent, value)) {
                    if (originalParent != null) {
                        originalParent.PropertyChanged -= this.Parent_PropertyChanged;
                        originalParent.RemoveChild(this);
                    }

                    if (this._parent != null) {
                        this._parent.PropertyChanged += this.Parent_PropertyChanged;
                        this._parent.AddChild(this);
                    }
                }
            }
        }

        public virtual AssetType Type {
            get {
                return AssetType.File;
            }
        }

        public virtual void BuildProcessorCommands(StringBuilder contentStringBuilder) {
            return;
        }

        public virtual string GetContentPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this.Parent.GetContentPath(), this.Name);
            }

            return (this.Name ?? string.Empty);
        }

        public virtual string GetPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this.Parent.GetPath(), this.Name);
            }

            return (this.Name ?? string.Empty);
        }

        public virtual void Refresh() {
            return;
        }

        public override string ToString() {
            return this.Name;
        }

        internal FolderAsset GetRootFolder() {
            var root = this;

            while (root.Parent is FolderAsset folderAsset) {
                root = folderAsset;
            }

            return root as FolderAsset;
        }

        private void Parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Parent)) {
                // Raise the parent property changed so that children know to refresh thier paths.
                this.RaisePropertyChanged(nameof(this.Parent));
            }
        }
    }
}