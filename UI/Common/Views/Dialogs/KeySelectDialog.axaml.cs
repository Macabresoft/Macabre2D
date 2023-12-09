namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Input;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.UI.AvaloniaInterop;
using Microsoft.Xna.Framework.Input;

public partial class KeySelectDialog : BaseDialog {
    public KeySelectDialog() : base() {
        this.InitializeComponent();
    }

    public Keys? SelectedKey { get; private set; }
    
    private void OnKeyDown(object _, KeyEventArgs e) {
        if (e.Key.TryConvertKey(out var monoGameKey)) {
            this.SelectedKey = monoGameKey;
            this.Close(true);
        }
    }
}