namespace Macabre2D.UI.Library.Views.Data {

    using Macabre2D.UI.Library.Models;
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