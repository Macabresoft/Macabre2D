namespace Macabre2D.UI.Services {

    using Macabre2D.UI.Controls.AssetEditors;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Windows;

    public sealed class AssetEditorService : IAssetEditorService {

        public DependencyObject GetEditor(Asset asset) {
            DependencyObject editor = null;
            if (asset is ImageAsset imageAsset) {
                editor = new ImageAssetEditor {
                    Asset = imageAsset
                };
            }
            else if (asset is SpriteWrapper spriteWrapper) {
                editor = new SpriteWrapperEditor {
                    ImageAsset = spriteWrapper.ImageAsset,
                    SpriteWrapper = spriteWrapper
                };
            }
            else if (asset is SpriteAnimationAsset spriteAnimation) {
                editor = new SpriteAnimationAssetEditor {
                    Asset = spriteAnimation
                };
            }
            else if (asset is FontAsset fontAsset) {
                editor = new FontAssetEditor {
                    Asset = fontAsset
                };
            }

            return editor;
        }
    }
}