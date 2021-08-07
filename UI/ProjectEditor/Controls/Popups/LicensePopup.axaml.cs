namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.Popups {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models;

    public class LicensePopup : UserControl, IPopup {
        public LicensePopup() {
            this.InitializeComponent();
        }

        public string Header => "License";

        public bool TryClose() {
            return true;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}