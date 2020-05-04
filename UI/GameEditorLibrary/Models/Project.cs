namespace Macabre2D.UI.GameEditorLibrary.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models.Validation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Project : ValidationModel {
        public const string ProjectFileName = nameof(Project) + FileHelper.ProjectExtension;

        [DataMember]
        private readonly ProjectAsset _assetFolder;

        private readonly List<BuildConfiguration> _buildConfigurations = new List<BuildConfiguration>();

        [DataMember]
        private readonly Version _version = new Version();

        private string _projectDirectoryPath;
        private SceneAsset _startUpSceneAsset;

        public Project() {
            this._assetFolder = new ProjectAsset(this);
            this._assetFolder.PropertyChanged += this.AssetFolder_PropertyChanged;
            this._assetFolder.OnAssetAdded += this.AssetFolder_OnAssetAdded;
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
        }

        [DataMember]
        public AssetManager AssetManager { get; private set; } = new AssetManager();

        [DataMember]
        public IReadOnlyCollection<BuildConfiguration> BuildConfigurations {
            get {
                return this._buildConfigurations;
            }
        }

        public BuildConfiguration EditorConfiguration {
            get {
                return this.BuildConfigurations.FirstOrDefault(x => x.Platform == BuildPlatform.DesktopGL) ?? new BuildConfiguration(BuildPlatform.DesktopGL);
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
                return this.GameSettings.ProjectName;
            }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    this.GameSettings.ProjectName = value;
                    this.RaisePropertyChanged(nameof(this.Name));
                    this.RaisePropertyChanged(nameof(this.SafeName));
                }
                else {
                    throw new ArgumentNullException(nameof(this.Name), "Project name cannot be null.");
                }
            }
        }

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
                if (this.Set(ref this._startUpSceneAsset, value) && this._startUpSceneAsset != null) {
                    this.GameSettings.StartupSceneAssetId = this._startUpSceneAsset.Id;
                }
            }
        }

        public Version Version {
            get { return this._version; }
        }

        public void Refresh() {
            var startupId = this._startUpSceneAsset?.Id;
            var lastSceneId = this.LastSceneOpened?.Id;
            this.AssetManager.ClearMappings();
            this.AssetManager.Unload();
            this._assetFolder.Refresh();
            var nonFolderAssets = this._assetFolder.GetAllContentAssets();

            foreach (var asset in nonFolderAssets) {
                asset.Refresh(this.AssetManager);
                if (asset is MetadataAsset metadataAsset) {
                    metadataAsset.HasChanges = false;
                }
            }

            this.RaisePropertyChanged(nameof(this.AssetFolder));
            this.SceneAssets.Clear();

            foreach (var asset in this._assetFolder.GetAssetsOfType<SceneAsset>()) {
                this.SceneAssets.Add(asset);
            }

            this.StartUpSceneAsset = startupId.HasValue ? this.SceneAssets.FirstOrDefault(x => x.Id == startupId.Value) : this.SceneAssets.FirstOrDefault();
            this.LastSceneOpened = lastSceneId.HasValue ? this.SceneAssets.FirstOrDefault(x => x.Id == lastSceneId.Value) : this.SceneAssets.FirstOrDefault();
        }

        public void SaveAssets() {
            var assets = this.AssetFolder.GetAssetsOfType<MetadataAsset>();
            foreach (var asset in assets) {
                asset.Save();
            }
        }

        internal void Initialize(string projectDirectoryPath) {
            this._projectDirectoryPath = projectDirectoryPath;
        }

        private void AssetFolder_OnAssetAdded(object sender, Asset asset) {
            if (asset is SceneAsset sceneAsset && !this.SceneAssets.Any(x => x.Id == sceneAsset.Id)) {
                this.SceneAssets.Add(sceneAsset);
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

            public Project Project {
                get {
                    return this._project;
                }
            }

            public override void Delete() {
                return; // You can't delete the project asset node.
            }

            public override string GetPath() {
                return Path.Combine(this._project._projectDirectoryPath, "Content", base.GetPath());
            }
        }
    }
}