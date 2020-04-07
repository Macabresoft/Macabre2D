namespace Macabre2D.UI.Library.Models {

    using Macabre2D.Framework;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;

    public abstract class AddableAsset : MetadataAsset {

        public AddableAsset() : this(string.Empty) {
        }

        public AddableAsset(string name) : base(name) {
        }

        public abstract string FileExtension { get; }
    }

    public abstract class AddableAsset<T> : AddableAsset, ISyncAsset<T> where T : IAsset, new() {

        [DataMember]
        private T _savableValue;

        public AddableAsset() : this(string.Empty) {
        }

        public AddableAsset(string name) : base(name) {
        }

        public T SavableValue {
            get {
                if (this._savableValue == null) {
                    this._savableValue = this.GetInitialSavableValue();
                }

                return this._savableValue;
            }
        }

        public IEnumerable<T> GetAssetsToSync() {
            return new[] { this.SavableValue };
        }

        public override void Refresh(AssetManager assetManager) {
            this.SavableValue.AssetId = this.Id;

            if (this.IsContent) {
                assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
            }

            this._savableValue = this.GetInitialSavableValue();
            base.Refresh(assetManager);
        }

        protected virtual T CreateAsset() {
            return new T();
        }

        protected virtual T GetInitialSavableValue() {
            var result = this._savableValue;

            if (result == null && this.IsContent) {
                var path = this.GetPath();

                if (File.Exists(path)) {
                    result = Serializer.Instance.Deserialize<T>(this.GetPath());
                }
            }

            if (result == null) {
                result = this.CreateAsset();
            }

            return result;
        }

        protected override void SaveChanges() {
            this.SavableValue.AssetId = this.Id;

            if (this.IsContent) {
                Serializer.Instance.Serialize(this.SavableValue, this.GetPath());
            }
        }
    }
}