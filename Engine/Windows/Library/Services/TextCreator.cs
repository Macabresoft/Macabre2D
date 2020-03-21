namespace Macabre2D.Engine.Windows.Services {

    using System.IO;

    public interface ITextCreator {

        string Create();

        void CreateAndSave(string path, bool forceCreation);
    }

    public abstract class TextCreator : ITextCreator {

        public abstract string Create();

        public virtual void CreateAndSave(string path, bool forceCreation) {
            var shouldCreate = true;

            if (File.Exists(path)) {
                if (forceCreation) {
                    File.Delete(path);
                }
                else {
                    shouldCreate = false;
                }
            }

            if (shouldCreate) {
                var text = this.Create();
                File.WriteAllText(path, text);
            }
        }
    }
}