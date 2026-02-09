namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class LicenseDialog : BaseDialog {
    private readonly ObservableCollectionExtended<LicenseDefinition> _filteredLicenses = new();

    [InjectionConstructor]
    public LicenseDialog() : base() {
        this.CollapseCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(false));
        this.ExpandCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(true));

        this.FilterLicenses(string.Empty);
        this.InitializeComponent();
    }

    public ICommand CollapseCommand { get; }

    public ICommand ExpandCommand { get; }

    public IReadOnlyCollection<LicenseDefinition> FilteredLicenses => this._filteredLicenses;

    public IReadOnlyCollection<LicenseDefinition> Licenses => LicenseHelper.Definitions;

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (this.Find<IInputElement>("_filterBox") is { } filterBox) {
            filterBox.Focus();
        }
    }

    private void AdjustGroupBoxes(bool showContent) {
        foreach (var groupBox in this.GetVisualDescendants().OfType<GroupBox>()) {
            groupBox.ShowContent = showContent;
        }
    }

    private void AutoCompleteBox_OnTextChanged(object sender, TextChangedEventArgs e) {
        if (sender is AutoCompleteBox autoCompleteBox) {
            this.FilterLicenses(autoCompleteBox.Text);
        }
    }

    private void FilterLicenses(string filterText) {
        this._filteredLicenses.Reset(!string.IsNullOrEmpty(filterText) ? this.Licenses.Where(x => x.Product.Contains(filterText, StringComparison.OrdinalIgnoreCase)) : this.Licenses);
    }
}