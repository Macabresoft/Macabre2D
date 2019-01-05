namespace Macabre2D.UI.Controls {

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    public partial class IconToggleButton : UserControl {

        public static readonly DependencyProperty IsNotToggledIconProperty = DependencyProperty.Register(
            nameof(IsNotToggledIcon),
            typeof(Path),
            typeof(IconToggleButton),
            new PropertyMetadata());

        public static readonly DependencyProperty IsToggledIconProperty = DependencyProperty.Register(
            nameof(IsToggledIcon),
            typeof(Path),
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

        public Path IsNotToggledIcon {
            get { return (Path)this.GetValue(IsNotToggledIconProperty); }
            set { this.SetValue(IsNotToggledIconProperty, value); }
        }

        public bool IsToggled {
            get { return (bool)this.GetValue(IsToggledProperty); }
            set { this.SetValue(IsToggledProperty, value); }
        }

        public Path IsToggledIcon {
            get { return (Path)this.GetValue(IsToggledIconProperty); }
            set { this.SetValue(IsToggledIconProperty, value); }
        }
    }
}