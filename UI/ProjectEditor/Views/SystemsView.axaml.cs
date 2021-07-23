namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using System.Collections.Generic;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Helpers;

    public class SystemsView : UserControl {
        public SystemsView() {
            this.DataContext = Resolver.Resolve<SystemsViewModel>();
            this.InitializeComponent();
            this.AddMenuItems = MenuItemHelper.CreateAddMenuItems(this.ViewModel.SystemService.AvailableTypes, true);
        }

        public IReadOnlyCollection<IControl> AddMenuItems { get; }

        public SystemsViewModel ViewModel => this.DataContext as SystemsViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}