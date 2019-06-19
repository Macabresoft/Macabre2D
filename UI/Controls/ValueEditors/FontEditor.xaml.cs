namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using System.Windows;
    using System.Windows.Input;

    public partial class FontEditor : NamedValueEditor<Font> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(FontAsset),
            typeof(FontEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectFontCommandProperty = DependencyProperty.Register(
            nameof(SelectFontCommand),
            typeof(ICommand),
            typeof(FontEditor),
            new PropertyMetadata());

        public FontEditor() {
            this.InitializeComponent();
        }

        public FontAsset Asset {
            get { return (FontAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand SelectFontCommand {
            get { return (ICommand)this.GetValue(SelectFontCommandProperty); }
            set { this.SetValue(SelectFontCommandProperty, value); }
        }
    }
}