namespace Macabre2D.UI.Controls.AssetEditors {

    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SpriteWrapperEditor.xaml
    /// </summary>
    public partial class SpriteWrapperEditor : UserControl {

        public static readonly DependencyProperty ImageAssetProperty = DependencyProperty.Register(
            nameof(ImageAsset),
            typeof(ImageAsset),
            typeof(SpriteWrapperEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty SpriteWrapperProperty = DependencyProperty.Register(
            nameof(SpriteWrapper),
            typeof(SpriteWrapper),
            typeof(SpriteWrapperEditor),
            new PropertyMetadata());

        public SpriteWrapperEditor() {
            this.InitializeComponent();
        }

        public ImageAsset ImageAsset {
            get { return (ImageAsset)this.GetValue(ImageAssetProperty); }
            set { this.SetValue(ImageAssetProperty, value); }
        }

        public SpriteWrapper SpriteWrapper {
            get { return (SpriteWrapper)this.GetValue(SpriteWrapperProperty); }
            set { this.SetValue(SpriteWrapperProperty, value); }
        }
    }
}