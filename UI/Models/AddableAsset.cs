namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System;

    public abstract class AddableAsset : MetadataAsset {

        public AddableAsset() : base() {
        }

        public AddableAsset(string name) : base(name) {
        }

        public abstract string FileExtension { get; }

        public bool RequiresCreation { get; set; }
    }

    public abstract class AddableAsset<T> : AddableAsset where T : IAsset, new() {
        private ResettableLazy<T> _savableValue;

        public AddableAsset() : base() {
            this._savableValue = new ResettableLazy<T>(this.DeserializeSavableValue);
        }

        public AddableAsset(string name) : base(name) {
            this._savableValue = new ResettableLazy<T>(this.DeserializeSavableValue);
        }

        public T SavableValue {
            get {
                return this._savableValue.Value;
            }
        }

        public override void Refresh(AssetManager assetManager) {
            this.SavableValue.AssetId = this.Id;
            assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());

            if (this._savableValue.IsValueCreated && this.SavableValue is IDisposable disposable) {
                disposable.Dispose();
            }

            this._savableValue.Reset();
            base.Refresh(assetManager);
        }

        protected virtual T DeserializeSavableValue() {
            T result;

            if (this.RequiresCreation) {
                result = new T();
            }
            else {
                var serializer = new Serializer();
                result = serializer.Deserialize<T>(this.GetPath());
            }

            return result;
        }

        protected override void SaveChanges(Serializer serializer) {
            serializer.Serialize(this.SavableValue, this.GetPath());
            this.RequiresCreation = false;
        }
    }
}