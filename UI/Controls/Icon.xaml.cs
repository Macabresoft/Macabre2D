namespace Macabre2D.UI.Controls {

    using System.Windows;
    using System.Windows.Shapes;

    public partial class Icon {

        public static readonly DependencyProperty VectorPathProperty = DependencyProperty.Register(
            nameof(VectorPath),
            typeof(Path),
            typeof(Icon),
            new PropertyMetadata());

        public Icon() {
            InitializeComponent();
        }

        public Path VectorPath {
            get { return (Path)GetValue(VectorPathProperty); }
            set { SetValue(VectorPathProperty, value); }
        }
    }
}