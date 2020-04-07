namespace Macabre2D.UI.Library.Controls {

    using System.Windows;
    using System.Windows.Controls;

    public partial class ContentSwitcher : UserControl {

        public static readonly DependencyProperty FalseContentProperty = DependencyProperty.Register(
            nameof(FalseContent),
            typeof(FrameworkElement),
            typeof(ContentSwitcher),
            new PropertyMetadata());

        public static readonly DependencyProperty SwitchValueProperty = DependencyProperty.Register(
            nameof(SwitchValue),
            typeof(bool),
            typeof(ContentSwitcher),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TrueContentProperty = DependencyProperty.Register(
            nameof(TrueContent),
            typeof(FrameworkElement),
            typeof(ContentSwitcher),
            new PropertyMetadata());

        public ContentSwitcher() {
            InitializeComponent();
        }

        public FrameworkElement FalseContent {
            get { return (FrameworkElement)GetValue(FalseContentProperty); }
            set { SetValue(FalseContentProperty, value); }
        }

        public bool SwitchValue {
            get { return (bool)GetValue(SwitchValueProperty); }
            set { SetValue(SwitchValueProperty, value); }
        }

        public FrameworkElement TrueContent {
            get { return (FrameworkElement)GetValue(TrueContentProperty); }
            set { SetValue(TrueContentProperty, value); }
        }
    }
}