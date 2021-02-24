namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ImageContentFile : ContentFile<Texture2D> {
        public ImageContentFile(string name, ContentMetadata metadata) : base(name, metadata) {
        }
    }
}