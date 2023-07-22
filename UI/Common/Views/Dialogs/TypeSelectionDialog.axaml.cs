namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class TypeSelectionDialog : BaseDialog {
    public TypeSelectionDialog() : base() {
    }

    [InjectionConstructor]
    public TypeSelectionDialog(TypeSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }


    public TypeSelectionViewModel ViewModel => this.DataContext as TypeSelectionViewModel;

    private void AutoCompleteBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        var addedTypes = e.AddedItems.OfType<Type>().ToList();
        if (addedTypes.Count == 1) {
            this.ViewModel.SelectedType = addedTypes.First();
        }
    }
}