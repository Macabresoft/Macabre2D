namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;

    public abstract class AddableAsset : MetadataAsset {

        public AddableAsset() : base() {
        }

        public AddableAsset(string name) : base(name) {
        }

        public abstract string FileExtension { get; }
    }

    public abstract class AddableAsset<T> : AddableAsset where T : new() {

        public AddableAsset() : base() {
        }

        public AddableAsset(string name) : base(name) {
        }

        public T SavableValue { get; private set; } = new T();

        public override void Refresh(AssetManager assetManager) {
            var serializer = new Serializer();
            this.SavableValue = serializer.Deserialize<T>(this.GetPath());
        }

        protected override void SaveChanges(Serializer serializer) {
            serializer.Serialize(this.SavableValue, this.GetPath());
        }
    }
}