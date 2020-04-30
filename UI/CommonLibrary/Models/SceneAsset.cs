namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class SceneAsset : AddableAsset<Scene>, IParent<ComponentWrapper> {
        private readonly ObservableCollection<ComponentWrapper> _children = new ObservableCollection<ComponentWrapper>();

        [DataMember]
        private Camera _camera = new Camera();

        private bool _isLoading;

        public SceneAsset(string name) : base(name) {
            this._children.CollectionChanged += this.Children_CollectionChanged;
            this.Modules.CollectionChanged += this.Modules_CollectionChanged;
        }

        public SceneAsset() : this(string.Empty) {
        }

        public Color BackgroundColor {
            get {
                return this.SavableValue.BackgroundColor;
            }

            set {
                if (this.SavableValue.BackgroundColor != value) {
                    this.SavableValue.BackgroundColor = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Camera Camera {
            get {
                return this._camera;
            }
        }

        public IReadOnlyCollection<ComponentWrapper> Children {
            get {
                return this._children;
            }
        }

        public override string FileExtension {
            get {
                return FileHelper.SceneExtension;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FileCloud;
            }
        }

        public override bool IsContent {
            get {
                return true;
            }
        }

        public ObservableCollection<ModuleWrapper> Modules { get; } = new ObservableCollection<ModuleWrapper>();

        public bool AddChild(ComponentWrapper child) {
            var result = false;
            if (!this._children.Any(x => x.Id == child.Id)) {
                this._children.Add(child);
                child.Parent = this;
                result = true;
            }

            return result;
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder) {
            var path = this.GetContentPath();
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:SceneImporter");
            contentStringBuilder.AppendLine(@"/processor:SceneProcessor");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public void Load() {
            this.Unload();

            try {
                this._isLoading = true;

                foreach (var component in this.SavableValue.GetAllComponents(true, false)) {
                    this.AddChild(new ComponentWrapper(component));
                }

                foreach (var module in this.SavableValue.GetAllModules()) {
                    this.Modules.Add(new ModuleWrapper(module));
                }
            }
            finally {
                this._isLoading = false;
            }
        }

        public bool RemoveChild(ComponentWrapper child) {
            return this._children.Remove(child);
        }

        public override string ToString() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        public void Unload() {
            try {
                this._isLoading = true;
                this._children.CollectionChanged -= this.Children_CollectionChanged;
                this.Modules.CollectionChanged -= this.Modules_CollectionChanged;
                this._children.Clear();
                this.Modules.Clear();
                this._children.CollectionChanged += this.Children_CollectionChanged;
                this.Modules.CollectionChanged += this.Modules_CollectionChanged;
            }
            finally {
                this._isLoading = false;
            }
        }

        internal override void ResetContentPath() {
            base.ResetContentPath();

            if (this.SavableValue is Scene scene) {
                scene.Name = this.Name;
            }
        }

        protected override void SaveChanges() {
            this.SavableValue.SaveToFile(this.GetPath());
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var newItem in e.NewItems.OfType<ComponentWrapper>()) {
                    newItem.PropertyChanged += this.ComponentPropertyChanged;
                    this.SavableValue.AddComponent(newItem.Component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var oldItem in e.OldItems.OfType<ComponentWrapper>()) {
                    oldItem.PropertyChanged -= this.ComponentPropertyChanged;
                    if (oldItem.Component.Parent == null) {
                        this.SavableValue.RemoveComponent(oldItem.Component);
                    }
                    else {
                        this.SavableValue.RemoveChild(oldItem.Component);
                    }
                }
            }

            if (!this._isLoading) {
                this.RaisePropertyChanged(nameof(this.Children));
            }
        }

        private void ComponentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!this._isLoading) {
                this.RaisePropertyChanged(nameof(this.Children));
            }
        }

        private void ModulePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!this._isLoading) {
                this.RaisePropertyChanged(nameof(this.Modules));
            }
        }

        private void Modules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var newItem in e.NewItems.OfType<ModuleWrapper>()) {
                    newItem.PropertyChanged += this.ModulePropertyChanged;
                    this.SavableValue.AddModule(newItem.Module);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var oldItem in e.OldItems.OfType<ModuleWrapper>()) {
                    oldItem.PropertyChanged -= this.ModulePropertyChanged;
                    this.SavableValue.RemoveModule(oldItem.Module);
                }
            }

            if (!this._isLoading) {
                this.RaisePropertyChanged(nameof(this.Modules));
            }
        }
    }
}