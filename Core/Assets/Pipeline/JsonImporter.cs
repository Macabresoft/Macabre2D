namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using System.IO;

    /// <summary>
    /// Content importer for JSON.
    /// </summary>
    public class JsonImporter : ContentImporter<string> {

        /// <inheritdoc />
        public override string Import(string filename, ContentImporterContext context) {
            return File.ReadAllText(filename);
        }
    }
}