namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using System.Collections.Generic;
    using System.Windows.Input;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    public class LicenseDialog : Window {
        public LicenseDialog() {
            this.OkCommand = ReactiveCommand.Create(() => this.Close(true));
            this.InitializeComponent();
        }

        public IReadOnlyCollection<LicenseDefinition> Licenses => LicenseHelper.Definitions;
        
        public ICommand OkCommand { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}