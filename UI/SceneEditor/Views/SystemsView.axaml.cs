namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using System.Collections.Generic;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class SystemsView : UserControl {
        public SystemsView() {
            this.DataContext = Resolver.Resolve<SystemsBaseViewModel>();
            this.InitializeComponent();
            this.AddMenuItems = MenuItemHelper.CreateAddMenuItems(this.BaseViewModel.SystemService.AvailableTypes, true);
        }

        public IReadOnlyCollection<IControl> AddMenuItems { get; }

        public SystemsBaseViewModel BaseViewModel => this.DataContext as SystemsBaseViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}