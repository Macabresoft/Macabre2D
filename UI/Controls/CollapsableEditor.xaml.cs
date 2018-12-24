namespace Macabre2D.UI.Controls {

    using System.Windows;
    using System.Windows.Controls;

    public partial class CollapsableEditor : UserControl {

        public static readonly DependencyProperty CollapsedContentProperty = DependencyProperty.Register(
            nameof(CollapsedContent),
            typeof(DependencyObject),
            typeof(CollapsableEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(CollapsableEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(CollapsableEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty UncollapsedContentProperty = DependencyProperty.Register(
            nameof(UncollapsedContent),
            typeof(DependencyObject),
            typeof(CollapsableEditor),
            new PropertyMetadata());

        public CollapsableEditor() {
            InitializeComponent();
        }

        public DependencyObject CollapsedContent {
            get { return (DependencyObject)GetValue(CollapsedContentProperty); }
            set { SetValue(CollapsedContentProperty, value); }
        }

        public bool IsCollapsed {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public DependencyObject UncollapsedContent {
            get { return (DependencyObject)GetValue(UncollapsedContentProperty); }
            set { SetValue(UncollapsedContentProperty, value); }
        }
    }
}