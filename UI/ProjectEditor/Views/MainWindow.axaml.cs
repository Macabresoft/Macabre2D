namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Controls.Popups;
    using ReactiveUI;
    using Key = Avalonia.Remote.Protocol.Input.Key;

    public class MainWindow : Window {
        private bool _shouldClose;

        public MainWindow() : base() {
            this.ViewLicenseCommand = ReactiveCommand.Create(this.ViewLicense);
        }

        public ICommand ViewLicenseCommand { get; }

        public MainWindowViewModel ViewModel => this.DataContext as MainWindowViewModel;

        protected override async void OnClosing(CancelEventArgs e) {
            if (!this._shouldClose && this.ViewModel is MainWindowViewModel viewModel) {
                e.Cancel = true;

                if (await viewModel.TryClose() != YesNoCancelResult.Cancel) {
                    this._shouldClose = true;
                    this.Close();
                }
            }

            base.OnClosing(e);
        }

        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }

        private void TabControl_OnInitialized(object sender, EventArgs e) {
            if (sender is TabControl tabControl && this.ViewModel is MainWindowViewModel viewModel) {
                var itemToSelect = tabControl.Items.Cast<TabItem>().FirstOrDefault(x => x.Tag != null && (EditorTabs)x.Tag == viewModel.SelectedTab);
                if (itemToSelect != null) {
                    tabControl.SelectedItem = itemToSelect;
                }

                tabControl.Initialized -= this.TabControl_OnInitialized;
            }
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (sender is TabControl { SelectedItem: TabItem { Tag: EditorTabs header } }) {
                this.ViewModel.SelectedTab = header;
            }
        }

        private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            this.BeginMoveDrag(e);
        }

        private void ViewLicense() {
            if (this.ViewModel?.PopupService.ShowPopup<LicensePopup>(out _) == true) {
                this.Focus();
            }
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Avalonia.Input.Key.Escape && this.ViewModel.ClosePopupCommand.CanExecute(null)) {
                this.ViewModel.ClosePopupCommand.Execute(null);
            }
        }
    }
}