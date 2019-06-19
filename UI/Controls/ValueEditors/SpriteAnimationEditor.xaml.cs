namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteAnimationEditor : NamedValueEditor<SpriteAnimation> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(SpriteAnimationAsset),
            typeof(SpriteAnimationEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectSpriteAnimationCommandProperty = DependencyProperty.Register(
            nameof(SelectSpriteAnimationCommand),
            typeof(ICommand),
            typeof(SpriteAnimationEditor),
            new PropertyMetadata());

        public SpriteAnimationEditor() {
            this.InitializeComponent();
        }

        public SpriteAnimationAsset Asset {
            get { return (SpriteAnimationAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand SelectSpriteAnimationCommand {
            get { return (ICommand)this.GetValue(SelectSpriteAnimationCommandProperty); }
            set { this.SetValue(SelectSpriteAnimationCommandProperty, value); }
        }
    }
}