namespace Macabre2D.UI.GameEditorLibrary.Models {

    using Macabre2D.Framework;

    public class MetadataAsset : Asset {
        private bool _hasChanges;

        public MetadataAsset(string name) : base(name) {
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        public MetadataAsset() : this(string.Empty) {
        }

        public bool HasChanges {
            get { return this._hasChanges; }
            set { this.Set(ref this._hasChanges, value); }
        }

        public void ForceSave() {
            this.HasChanges = true;
            this.Save();
        }

        public void Save() {
            if (this.HasChanges) {
                try {
                    if (this.IsContent) {
                        AssetManager.Instance.SetMapping(this.Id, this.GetContentPathWithoutExtension());
                    }

                    this.SaveChanges();
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
            if (e.PropertyName != nameof(this.HasChanges)) {
                if (e.PropertyName == nameof(this.Name)) {
                    this.ResetContentPath();
                }

                this.HasChanges = true;
            }
        }
    }
}