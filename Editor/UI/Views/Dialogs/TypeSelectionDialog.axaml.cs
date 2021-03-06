﻿namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;
    using Unity;

    public class TypeSelectionDialog : Window {
        public TypeSelectionDialog() {
        }
        
        [InjectionConstructor]
        public TypeSelectionDialog(TypeSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += OnCloseRequested;
            this.InitializeComponent();
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }

        public TypeSelectionViewModel ViewModel => this.DataContext as TypeSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}