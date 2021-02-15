namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ImageContentFile : ContentFile<Texture2D> {
        private static readonly Type[] AvailableAssetTypes = {
            typeof(SpriteSheet)
        };

        public ImageContentFile(string name, ContentMetadata metadata) : base(name, metadata) {
        }
    }
}