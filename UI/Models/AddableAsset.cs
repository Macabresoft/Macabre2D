namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System.Runtime.Serialization;

    public abstract class AddableAsset : MetadataAsset {

        public AddableAsset() : this(string.Empty) {
        }

        public AddableAsset(string name) : base(name) {
        }

        public abstract string FileExtension { get; }

        [DataMember]
        public bool RequiresCreation { get; set; }
    }

    public abstract class AddableAsset<T> : AddableAsset where T : IAsset, new() {

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

        public override void Refresh(AssetManager assetManager) {
            this.SavableValue.AssetId = this.Id;
            assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
            this._savableValue = this.GetInitialSavableValue();
            base.Refresh(assetManager);
        }

        protected virtual T CreateAsset() {
            return new T();
        }

        protected virtual T GetInitialSavableValue() {
            var result = this._savableValue;

            if (this.RequiresCreation) {
                result = this.CreateAsset();
                this.RequiresCreation = false;
            }
            else if (result == null) {
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