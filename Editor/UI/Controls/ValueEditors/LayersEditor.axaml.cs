namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Avalonia.Data;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.UI.Converters;
    using Macabresoft.Macabre2D.Framework;

    public class LayersEditor : ValueEditorControl<Layers> {
        public LayersEditor() {
            this.InitializeComponent();
        }


        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
            base.OnAttachedToLogicalTree(e);

            var layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().OrderBy(x => (ushort)x).ToList();
            layers.Remove(Layers.None);
            layers.Remove(Layers.All);

            var itemsControl = this.Find<ItemsControl>("_layersItemsControl");
            var items = new List<CheckBox>();
            foreach (var layer in layers) {
                var checkBox = new CheckBox();
                var contentBinding = new Binding(nameof(Content));
                contentBinding.Source = layer;
                contentBinding.Path = string.Empty;
                checkBox.Bind(ContentProperty, contentBinding);
                var isCheckedBinding = new Binding(nameof(CheckBox.IsChecked)) {
                    Source = this,
                    Path = nameof(this.Value),
                    Converter = new LayersToBoolConverter(),
                    ConverterParameter = layer
                };

                checkBox.Bind(ToggleButton.IsCheckedProperty, isCheckedBinding);
                items.Add(checkBox);
            }

            itemsControl.Items = items;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}