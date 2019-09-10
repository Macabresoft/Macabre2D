namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

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

        public event EventHandler OnDeleted;

        public event EventHandler OnRefreshed;

        public string Extension {
            get {
                return Path.GetExtension(this.Name) ?? string.Empty;
            }
        }

        public virtual PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.File;
            }
        }

        [DataMember]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name {
            get {
                return this._name;
            }

            set {
                if (this.Set(ref this._name, value)) {
                    this.RaisePropertyChanged(nameof(this.NameWithoutExtension));
                }
            }
        }

        public string NameWithoutExtension {
            get {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(this.Name);
                if (string.IsNullOrWhiteSpace(nameWithoutExtension)) {
                    nameWithoutExtension = this.Name;
                }

                return nameWithoutExtension;
            }
        }

        public FolderAsset Parent {
            get {
                return this._parent;
            }

            set {
                var originalParent = this._parent as FolderAsset;
                var originalPath = this.GetPath();
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

        public virtual void BuildProcessorCommands(StringBuilder contentStringBuilder, string projectDirectoryPath) {
            return;
        }

        public virtual void Delete() {
            var path = this.GetPath();
            var fileAttributes = File.GetAttributes(path);
            if ((fileAttributes & FileAttributes.Directory) != FileAttributes.Directory) {
                File.Delete(path);
                this.RaiseOnDeleted();
            }
        }

        public virtual string GetContentPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this.Parent.GetContentPath(), this.Name);
            }

            return this.Name ?? string.Empty;
        }

        public virtual string GetPath() {
            if (this.Parent != null && !string.IsNullOrEmpty(this.Name)) {
                return Path.Combine(this.Parent.GetPath(), this.Name);
            }

            return (this.Name ?? string.Empty);
        }

        public virtual void Refresh(AssetManager assetManager) {
            this.RaiseOnRefreshed();
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

        internal virtual void ResetContentPath() {
            return;
        }

        protected string GetContentPathWithoutExtension() {
            return Path.ChangeExtension(this.GetContentPath(), null);
        }

        protected void RaiseOnDeleted() {
            this.OnDeleted?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseOnRefreshed() {
            this.OnRefreshed.SafeInvoke(this);
        }

        protected void RemoveIdentifiableContentFromScenes(Guid id) {
            var projectAsset = this.GetRootFolder() as Project.ProjectAsset;

            if (projectAsset != null) {
                var sceneAssets = projectAsset.GetAssetsOfType<SceneAsset>();

                foreach (var sceneAsset in sceneAssets) {
                    sceneAsset.Refresh(projectAsset.Project?.AssetManager);
                    var scene = sceneAsset.SavableValue;
                    var contentAssets = scene.GetAllComponentsOfType<IAssetComponent>();

                    foreach (var contentAsset in contentAssets.Where(x => x.HasAsset(id))) {
                        contentAsset.RemoveAsset(id);
                        sceneAsset.HasChanges = true;
                    }
                }
            }
        }

        private void Parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Parent)) {
                // Raise the parent property changed so that children know to refresh thier paths.
                this.RaisePropertyChanged(nameof(this.Parent));
                this.ResetContentPath();
            }
            else if (e.PropertyName == nameof(this.Name)) {
                this.ResetContentPath();

                if (this is FolderAsset) {
                    this.RaisePropertyChanged(nameof(this.Name));
                }
            }
        }
    }
}