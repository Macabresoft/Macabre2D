namespace Macabre2D.UI.Views.Dialogs {

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class OKCancelButtonsControl : UserControl {

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(OKCancelButtonsControl), new PropertyMetadata());

        public static readonly DependencyProperty OKCommandProperty =
            DependencyProperty.Register(nameof(OKCommand), typeof(ICommand), typeof(OKCancelButtonsControl), new PropertyMetadata());

        public OKCancelButtonsControl() {
            this.InitializeComponent();
        }

        public ICommand CancelCommand {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public ICommand OKCommand {
            get { return (ICommand)GetValue(OKCommandProperty); }
            set { SetValue(OKCommandProperty, value); }
        }
    }
}