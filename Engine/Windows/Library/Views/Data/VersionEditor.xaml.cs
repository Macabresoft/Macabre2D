namespace Macabre2D.Engine.Windows.Views.Data {

    using Macabre2D.Engine.Windows.Models;
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