namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System;
    using System.Runtime.Serialization;

    public abstract class AddableAsset : MetadataAsset {

        public AddableAsset() : base() {
        }

        public AddableAsset(string name) : base(name) {
        }

        public abstract string FileExtension { get; }

        [DataMember]
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

        protected virtual T CreateAsset() {
            return new T();
        }

        protected virtual T DeserializeSavableValue() {
            T result;

            if (this.RequiresCreation) {
                result = this.CreateAsset();
                this.RequiresCreation = false;
            }
            else {
                result = Serializer.Instance.Deserialize<T>(this.GetPath());
            }

            return result;
        }

        protected override void SaveChanges() {
            this.SavableValue.AssetId = this.Id;
            Serializer.Instance.Serialize(this.SavableValue, this.GetPath());
            this.RequiresCreation = false;
        }
    }
}