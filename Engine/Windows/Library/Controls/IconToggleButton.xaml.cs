namespace Macabre2D.Engine.Windows.Controls {

    using MahApps.Metro.IconPacks;
    using System.Windows;
    using System.Windows.Controls;

    public partial class IconToggleButton : UserControl {

        public static readonly DependencyProperty IsNotToggledKindProperty = DependencyProperty.Register(
            nameof(IsNotToggledKind),
            typeof(PackIconMaterialKind),
            typeof(IconToggleButton),
            new PropertyMetadata());

        public static readonly DependencyProperty IsToggledKindProperty = DependencyProperty.Register(
            nameof(IsToggledKind),
            typeof(PackIconMaterialKind),
            typeof(IconToggleButton),
            new PropertyMetadata());

        public static readonly DependencyProperty IsToggledProperty = DependencyProperty.Register(
            nameof(IsToggled),
            typeof(bool),
            typeof(IconToggleButton),
            new PropertyMetadata(false));

        public IconToggleButton() {
            this.InitializeComponent();
        }

        public PackIconMaterialKind IsNotToggledKind {
            get { return (PackIconMaterialKind)this.GetValue(IsNotToggledKindProperty); }
            set { this.SetValue(IsNotToggledKindProperty, value); }
        }

        public bool IsToggled {
            get { return (bool)this.GetValue(IsToggledProperty); }
            set { this.SetValue(IsToggledProperty, value); }
        }

        public PackIconMaterialKind IsToggledKind {
            get { return (PackIconMaterialKind)this.GetValue(IsToggledKindProperty); }
            set { this.SetValue(IsToggledKindProperty, value); }
        }
    }
}