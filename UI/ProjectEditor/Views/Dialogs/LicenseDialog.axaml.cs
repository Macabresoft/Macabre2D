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
            this.CollapseCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(false));
            this.ExpandCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(true));
            this.OkCommand = ReactiveCommand.Create(() => this.Close(true));
            this.FilterLicenses(string.Empty);
            this.InitializeComponent();
        }

        public ICommand CollapseCommand { get; }

        public ICommand ExpandCommand { get; }

        public IReadOnlyCollection<LicenseDefinition> FilteredLicenses => this._filteredLicenses;

        public IReadOnlyCollection<LicenseDefinition> Licenses => LicenseHelper.Definitions;

        public ICommand OkCommand { get; }

        private void AdjustGroupBoxes(bool showContent) {
            foreach (var license in this.Licenses) {
                license.IsExpanded = showContent;
            }
        }

        private void AutoCompleteBox_OnTextChanged(object sender, EventArgs e) {
            if (sender is AutoCompleteBox autoCompleteBox) {
                this.FilterLicenses(autoCompleteBox.Text);
            }
        }

        private void FilterLicenses(string filterText) {
            this._filteredLicenses.Reset(!string.IsNullOrEmpty(filterText) ? this.Licenses.Where(x => x.Product.Contains(filterText, StringComparison.OrdinalIgnoreCase)) : this.Licenses);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}