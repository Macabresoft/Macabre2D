namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    public class LicenseDialog : Window {
        private readonly ObservableCollectionExtended<LicenseDefinition> _filteredLicenses = new();

        public LicenseDialog() {
            this.OkCommand = ReactiveCommand.Create(() => this.Close(true));
            this.FilterLicenses(string.Empty);
            this.InitializeComponent();
        }

        public IReadOnlyCollection<LicenseDefinition> FilteredLicenses => this._filteredLicenses;

        public IReadOnlyCollection<LicenseDefinition> Licenses => LicenseHelper.Definitions;

        public ICommand OkCommand { get; }

        private void FilterLicenses(string filterText) {
            this._filteredLicenses.Reset(!string.IsNullOrEmpty(filterText) ? 
                this.Licenses.Where(x => x.Product.Contains(filterText, StringComparison.OrdinalIgnoreCase)) :
                this.Licenses);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void AutoCompleteBox_OnTextChanged(object sender, EventArgs e) {
            if (sender is AutoCompleteBox autoCompleteBox) {
                this.FilterLicenses(autoCompleteBox.Text);
            }
        }
    }
}