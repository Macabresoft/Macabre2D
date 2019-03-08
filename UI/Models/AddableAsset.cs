namespace Macabre2D.UI.Models {

    using Macabre2D.Framework.Serialization;

    public abstract class AddableAsset : MetadataAsset {
        public abstract string FileExtension { get; }
    }

    public abstract class AddableAsset<T> : AddableAsset where T : new() {
        public T SavableValue { get; private set; } = new T();

        public override void Refresh() {
            var serializer = new Serializer();
            this.SavableValue = serializer.Deserialize<T>(this.GetPath());
        }

        protected override void SaveChanges(Serializer serializer) {
            serializer.Serialize(this.SavableValue, this.GetPath());
        }
    }
}