namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ImageContentFile : ContentFile<Texture2D> {
        private static readonly Type[] AvailableAssetTypes = {
            typeof(SpriteSheet)
        };
        
        public ImageContentFile(string name, ContentMetadata metadata) : base(name, metadata) {
        }

        public override bool AddAsset(Type type) {
            return false;
        }

        public override IEnumerable<Type> GetCreatableAssetTypes() {
            return AvailableAssetTypes;
        }
    }
}