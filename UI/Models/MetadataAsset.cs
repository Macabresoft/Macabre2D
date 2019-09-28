namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using System.IO;

    public class MetadataAsset : Asset {

        public MetadataAsset(string name) : base(name) {
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        public MetadataAsset() : this(string.Empty) {
        }

        public bool HasChanges {
            get;
            internal set;
        }

        public string MetadataFileName {
            get {
                return $"{this.Name}{FileHelper.MetaDataExtension}";
            }
        }

        public static string GetMetadataPath(string assetPath) {
            return $"{assetPath}{FileHelper.MetaDataExtension}";
        }

        public override void Delete() {
            File.Delete(this.GetPath());
            File.Delete(this.GetMetadataPath());
            this.RaiseOnDeleted();
        }

        public string GetMetadataPath() {
            return MetadataAsset.GetMetadataPath(this.GetPath());
        }

        public void Save(AssetManager assetManager) {
            if (this.HasChanges) {
                try {
                    assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
                    this.SaveChanges();
                    Serializer.Instance.Serialize(this, this.GetMetadataPath());
                }
                finally {
                    this.HasChanges = false;
                }
            }
        }

        internal override void ResetContentPath() {
            if (this.GetRootFolder() is Project.ProjectAsset projectAsset) {
                projectAsset.Project?.AssetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
            }
        }

        protected virtual void SaveChanges() {
            return;
        }

        private void Self_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Name)) {
                this.ResetContentPath();
            }

            this.HasChanges = true;
        }
    }
}