namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using System.IO;

    /// <summary>
    /// Content importer for JSON.
    /// </summary>
    /// <seealso cref="Microsoft.Xna.Framework.Content.Pipeline.ContentImporter{string}"/>
    public class JsonImporter : ContentImporter<string> {

        /// <inheritdoc/>
        public override string Import(string filename, ContentImporterContext context) {
            return File.ReadAllText(filename);
        }
    }
}