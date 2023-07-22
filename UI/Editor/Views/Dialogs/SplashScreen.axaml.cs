namespace Macabresoft.Macabre2D.UI.Editor.Views.Dialogs;

using Avalonia.Controls;
using Avalonia.Input;

public partial class SplashScreen : Window {
    public SplashScreen() : base() {
        this.InitializeComponent();
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e) {
        this.BeginMoveDrag(e);
    }
}