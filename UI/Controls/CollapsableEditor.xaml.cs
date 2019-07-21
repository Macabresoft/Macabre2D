namespace Macabre2D.UI.Controls {

    using Macabre2D.UI.Controls.ValueEditors;
    using System.Windows;
    using System.Windows.Controls;

    public partial class CollapsableEditor : UserControl, ISeparatedValueEditor {

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

        public static readonly DependencyProperty ShowBottomSeparatorProperty = DependencyProperty.Register(
            nameof(ShowBottomSeparator),
            typeof(bool),
            typeof(CollapsableEditor),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTopSeparatorProperty = DependencyProperty.Register(
            nameof(ShowTopSeparator),
            typeof(bool),
            typeof(CollapsableEditor),
            new PropertyMetadata(true));

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
            this.InitializeComponent();
        }

        public DependencyObject CollapsedContent {
            get { return (DependencyObject)this.GetValue(CollapsedContentProperty); }
            set { this.SetValue(CollapsedContentProperty, value); }
        }

        public bool IsCollapsed {
            get { return (bool)this.GetValue(IsCollapsedProperty); }
            set { this.SetValue(IsCollapsedProperty, value); }
        }

        public bool ShowBottomSeparator {
            get { return (bool)this.GetValue(ShowBottomSeparatorProperty); }
            set { this.SetValue(ShowBottomSeparatorProperty, value); }
        }

        public bool ShowTopSeparator {
            get { return (bool)this.GetValue(ShowTopSeparatorProperty); }
            set { this.SetValue(ShowTopSeparatorProperty, value); }
        }

        public string Title {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public DependencyObject UncollapsedContent {
            get { return (DependencyObject)this.GetValue(UncollapsedContentProperty); }
            set { this.SetValue(UncollapsedContentProperty, value); }
        }
    }
}