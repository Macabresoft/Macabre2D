namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls {
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models;

    public class ValueEditorListControl : UserControl {
        public static readonly StyledProperty<IEnumerable<ValueEditorCollection>> CollectionsProperty =
            AvaloniaProperty.Register<ValueEditorListControl, IEnumerable<ValueEditorCollection>>(nameof(Collections), Enumerable.Empty<ValueEditorCollection>());

        public static readonly StyledProperty<bool> IsBusyProperty =
            AvaloniaProperty.Register<ValueEditorListControl, bool>(nameof(IsBusy));

        public ValueEditorListControl() {
            this.InitializeComponent();
        }

        public IEnumerable<ValueEditorCollection> Collections {
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