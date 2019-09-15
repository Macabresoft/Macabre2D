namespace Macabre2D.UI.Editor {

    using System.Windows;

    public partial class DraggableSplashScreen {

        public static readonly DependencyProperty ProgressTextProperty = DependencyProperty.Register(
            nameof(ProgressText),
            typeof(string),
            typeof(DraggableSplashScreen),
            new PropertyMetadata(string.Empty));

        public DraggableSplashScreen() {
            this.InitializeComponent();
        }

        public string ProgressText {
            get { return (string)this.GetValue(ProgressTextProperty); }
            set { this.SetValue(ProgressTextProperty, value); }
        }
    }
}