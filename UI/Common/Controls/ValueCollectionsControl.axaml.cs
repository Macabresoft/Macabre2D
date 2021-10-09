namespace Macabresoft.Macabre2D.UI.Common {
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ValueCollectionsControl : UserControl {
        public static readonly StyledProperty<IEnumerable<ValueControlCollection>> CollectionsProperty =
            AvaloniaProperty.Register<ValueCollectionsControl, IEnumerable<ValueControlCollection>>(nameof(Collections), Enumerable.Empty<ValueControlCollection>());

        public static readonly StyledProperty<bool> IsBusyProperty =
            AvaloniaProperty.Register<ValueCollectionsControl, bool>(nameof(IsBusy));

        public ValueCollectionsControl() {
            this.InitializeComponent();
        }

        public IEnumerable<ValueControlCollection> Collections {
            get => this.GetValue(CollectionsProperty);
            set => this.SetValue(CollectionsProperty, value);
        }

        public bool IsBusy {
            get => this.GetValue(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}