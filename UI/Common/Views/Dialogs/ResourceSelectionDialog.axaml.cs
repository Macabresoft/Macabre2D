namespace Macabresoft.Macabre2D.UI.Common;

using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class ResourceSelectionDialog : BaseDialog {
    public ResourceSelectionDialog() : base() {
    }

    [InjectionConstructor]
    public ResourceSelectionDialog(ResourceSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public ResourceSelectionViewModel ViewModel => this.DataContext as ResourceSelectionViewModel;

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (this.Find<IInputElement>("_filterBox") is { } filterBox) {
            filterBox.Focus();
        }
    }

    private void AutoCompleteBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        var addedResources = e.AddedItems.OfType<ResourceEntry>().ToList();
        if (addedResources.Count == 1) {
            this.ViewModel.SelectedItem = addedResources.First();
        }
    }
}