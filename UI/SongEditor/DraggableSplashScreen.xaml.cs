namespace Macabre2D.UI.SongEditor {

    using System.Reflection;
    using System.Windows.Input;

    public partial class DraggableSplashScreen {

        public DraggableSplashScreen() {
            this.InitializeComponent();
        }

        public string Version {
            get {
                var version = string.Empty;
                try {
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch {
                    // Who cares?
                }

                return version;
            }
        }

        private void SplashScreen_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }
    }
}