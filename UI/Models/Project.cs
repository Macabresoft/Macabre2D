namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Models.Validation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Project : ValidationModel {
        private readonly List<BuildConfiguration> _buildConfigurations = new List<BuildConfiguration>();

        [DataMember]
        private readonly Version _version = new Version();

        private ProjectAsset _assetFolder;

        [DataMember]
        private string _name = "Project Name";

        private SceneAsset _startUpSceneAsset;

        public Project() {
            this._assetFolder = new ProjectAsset(this);
            this._assetFolder.PropertyChanged += this.AssetFolder_PropertyChanged;
        }

        public Project(params BuildPlatform[] platforms) : this() {
            foreach (var platform in platforms) {
                if (this._buildConfigurations.All(x => x.Platform != platform)) {
                    this._buildConfigurations.Add(new BuildConfiguration(platform));
                }
            }
        }

        public FolderAsset AssetFolder {
            get {
                return this._assetFolder;
            }

            private set {
                var oldValue = this._assetFolder;
                if (value is ProjectAsset projectAsset && !projectAsset.Equals(oldValue)) {
                    this._assetFolder = projectAsset;
                    this._assetFolder.PropertyChanged += this.AssetFolder_PropertyChanged;

                    if (oldValue != null) {
                        oldValue.PropertyChanged -= this.AssetFolder_PropertyChanged;
                    }
                }
            }
        }

        [DataMember]
        public IReadOnlyCollection<BuildConfiguration> BuildConfigurations {
            get {
                return this._buildConfigurations;
            }
        }

        public string Directory {
            get {
                return Path.GetDirectoryName(this.PathToProject);
            }
        }

        [DataMember]
        public GameSettings GameSettings { get; } = new GameSettings();

        [DataMember]
        public SceneAsset LastSceneOpened { get; set; }

        [DataMember]
        public DateTime LastTimeSaved { get; set; } = DateTime.Now;

        [RequiredValidation(FieldName = "Name")]
        public string Name {
            get {
                return this._name;
            }
            set {
                if (this.Set(ref this._name, value)) {
                    this.RaisePropertyChanged(nameof(this.SafeName));
                }
            }
        }

        [DataMember]
        public string PathToProject { get; set; }

        public string SafeName {
            get {
                return this.Name?.ToSafeString();
            }
        }

        public ObservableCollection<SceneAsset> SceneAssets { get; } = new ObservableCollection<SceneAsset>();

        [DataMember]
        public SceneAsset StartUpSceneAsset {
            get {
                return this._startUpSceneAsset;
            }
            set {
                this.Set(ref this._startUpSceneAsset, value);
            }
        }

        public Version Version {
            get { return this._version; }
        }

        public void Refresh() {
            var startupId = this._startUpSceneAsset?.Id;
            var lastSceneId = this.LastSceneOpened?.Id;

            this._assetFolder.Refresh();
            var nonFolderAssets = this._assetFolder.GetAllContentAssets();

            foreach (var asset in nonFolderAssets) {
                asset.Refresh();
                if (asset is MetadataAsset metadataAsset) {
                    metadataAsset.HasChanges = false;
                }
            }

            this.RaisePropertyChanged(nameof(this.AssetFolder));
            this.SceneAssets.Clear();

            foreach (var asset in this._assetFolder.GetAssetsOfType<SceneAsset>()) {
                this.SceneAssets.Add(asset);
            }

            this.StartUpSceneAsset = this.SceneAssets.FirstOrDefault(x => x.Id == startupId.Value);

            if (lastSceneId.HasValue) {
                this.LastSceneOpened = this.SceneAssets.FirstOrDefault(x => x.Id == lastSceneId.Value);
            }
            else {
                this.LastSceneOpened = this.SceneAssets.FirstOrDefault();
            }
        }

        public void SaveAssets() {
            var serializer = new Serializer();
            var assets = this.AssetFolder.GetAssetsOfType<MetadataAsset>();
            foreach (var asset in assets) {
                asset.Save(serializer);
            }
        }

        private void AssetFolder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.AssetFolder));
        }

        internal class ProjectAsset : FolderAsset {

            [DataMember]
            private readonly Project _project;

            public ProjectAsset(Project project) : base("Assets") {
                this._project = project;
            }

            internal ProjectAsset() : base("Assets") {
            }

            public override void Delete() {
                return; // You can't delete the project asset node.
            }

            public override string GetContentPath() {
                return string.Empty;
            }

            public override string GetPath() {
                return Path.Combine(this._project.Directory, base.GetPath());
            }
        }
    }
}