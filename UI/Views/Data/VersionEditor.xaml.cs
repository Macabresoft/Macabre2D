namespace Macabre2D.UI.Views.Data {

    using Macabre2D.UI.Models;
    using System.Windows;
    using System.Windows.Controls;

    public partial class VersionEditor : UserControl {

        public static DependencyProperty VersionProperty =
            DependencyProperty.Register(nameof(Version), typeof(ObservableVersion), typeof(VersionEditor));

        public VersionEditor() {
            InitializeComponent();
        }

        public ObservableVersion Version {
            get {
                return this.GetValue(VersionProperty) as ObservableVersion;
            }

            set {
                this.SetValue(VersionProperty, value);
            }
        }
    }
}