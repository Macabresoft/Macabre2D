namespace Macabre2D.UI.Models
{
    using Macabre2D.Framework;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Flags]
    public enum AssetType
    {
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
    public class Asset : NotifyPropertyChanged
    {
        [DataMember]
        private string _name;

        private FolderAsset _parent;

        public Asset(string name)
        {
            this._name = name;
        }

        internal Asset()
        {
        }

        public event EventHandler OnDeleted;

        [DataMember]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name {
            get {
                return this._name;
            }

            set {
                var originalPath = this.GetPath();
                if (this.Set(ref this._name, value))
                {
                    this.MoveAsset(originalPath, this.GetPath());
                }
            }
        }

        public FolderAsset Parent {
            get {
                return this._parent;
            }

            set {
                var originalParent = this._parent as FolderAsset;
                var originalPath = this.GetPath();
                if (this.Set(ref this._parent, value))
                {
                    if (originalParent != null)
                    {
                        originalParent.PropertyChanged -= this.Parent_PropertyChanged;
                        originalParent.RemoveChild(this);
                    }

                    if (this._parent != null)
                    {
                        this._parent.PropertyChanged += this.Parent_PropertyChanged;
                        this._parent.AddChild(this);
                        this.MoveAsset(originalPath, this.GetPath());
                    }
                }
            }
        }

        public virtual AssetType Type {
            get {
                return AssetType.File;
            }
        }

        public virtual void BuildProcessorCommands(StringBuilder contentStringBuilder)
        {
            return;
        }

        public virtual void Delete()
        {
            var path = this.GetPath();
            var fileAttributes = File.GetAttributes(path);
            if ((fileAttributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                File.Delete(path);
                this.RaiseOnDeleted();
            }
        }

        public virtual string GetContentPath()
        {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name))
            {
                return Path.Combine(this.Parent.GetContentPath(), this.Name);
            }

            return (this.Name ?? string.Empty);
        }

        public virtual string GetPath()
        {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name))
            {
                return Path.Combine(this.Parent.GetPath(), this.Name);
            }

            return (this.Name ?? string.Empty);
        }

        public virtual void Refresh()
        {
            return;
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal FolderAsset GetRootFolder()
        {
            var root = this;

            while (root.Parent is FolderAsset folderAsset)
            {
                root = folderAsset;
            }

            return root as FolderAsset;
        }

        internal virtual void MoveAsset(string originalPath, string newPath)
        {
            if (!string.IsNullOrEmpty(originalPath) && !string.IsNullOrEmpty(newPath) && File.Exists(originalPath))
            {
                File.Move(originalPath, newPath);
                this.ResetContentPath(newPath);
            }
        }

        protected void RaiseOnDeleted()
        {
            this.OnDeleted?.Invoke(this, EventArgs.Empty);
        }

        protected void RemoveIdentifiableContentFromScenes(Guid id)
        {
            var projectAsset = this.GetRootFolder();

            if (projectAsset != null)
            {
                var sceneAssets = projectAsset.GetAssetsOfType<SceneAsset>();

                foreach (var sceneAsset in sceneAssets)
                {
                    var scene = sceneAsset.Load();
                    var contentAssets = scene.GetAllComponentsOfType<IIdentifiableContentComponent>();

                    foreach (var contentAsset in contentAssets.Where(x => x.HasContent(id)))
                    {
                        contentAsset.RemoveContent(id);
                    }
                }
            }
        }

        protected virtual void ResetContentPath(string newPath)
        {
            return;
        }

        private void Parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Parent))
            {
                // Raise the parent property changed so that children know to refresh thier paths.
                this.RaisePropertyChanged(nameof(this.Parent));
                this.ResetContentPath(this.GetPath());
            }
            else if (e.PropertyName == nameof(this.Name))
            {
                this.ResetContentPath(this.GetPath());

                if (this is FolderAsset)
                {
                    this.RaisePropertyChanged(nameof(this.Name));
                }
            }
        }
    }
}