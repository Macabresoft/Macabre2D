namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using MahApps.Metro.IconPacks;
    using Microsoft.Xna.Framework;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class SceneAsset : AddableAsset<Scene> {

        [DataMember]
        private Camera _camera = new Camera();

        private bool _isLoading;

        public SceneAsset(string name) : base(name) {
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

                foreach (var module in this.SavableValue.GetAllModules()) {
                    this.Modules.Add(new ModuleWrapper(module));
                }
            }
            finally {
                this._isLoading = false;
            }
        }

        public override string ToString() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        public void Unload() {
            try {
                this._isLoading = true;
                this.Modules.CollectionChanged -= this.Modules_CollectionChanged;
                this.Modules.Clear();
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