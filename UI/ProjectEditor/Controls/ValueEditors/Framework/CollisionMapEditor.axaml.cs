namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using System;
    using System.Globalization;
    using System.Linq;
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Converters;
    using Unity;

    public class CollisionMapEditor : ValueEditorControl<CollisionMap> {
        private readonly ToDisplayNameConverter _displayNameConverter = new();

        [InjectionConstructor]
        public CollisionMapEditor() {
            this.InitializeComponent();
            this.CreateMap();
        }

        private void CreateMap() {
            var viewBox = this.LogicalChildren.OfType<Viewbox>().First();
            var grid = new Grid();
            viewBox.Child = grid;

            var rowValues = Enum.GetValues<Layers>().ToList();
            rowValues.Remove(Layers.Default);
            rowValues.Remove(Layers.None);
            var columnValues = rowValues.ToList();

            for (var i = 0; i <= rowValues.Count + 1; i++) {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            }

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var row = 1;
            var column = columnValues.Count + 1;

            foreach (var columnValue in columnValues) {
                var textControl = new TextBlock {
                    Text = this._displayNameConverter.Convert(columnValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                    [ToolTip.TipProperty] = columnValue
                };

                textControl.Classes.Add("SmallLabel");

                var container = new Decorator {
                    Child = textControl
                };

                var layoutTransform = new LayoutTransformControl {
                    ClipToBounds = false,
                    LayoutTransform = new RotateTransform(90),
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = column,
                    Child = container
                };

                grid.Children.Add(layoutTransform);
                column--;
            }

            foreach (var rowValue in rowValues) {
                column = columnValues.Count + 1;

                var textControl = new TextBlock {
                    Text = this._displayNameConverter.Convert(rowValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                    [ToolTip.TipProperty] = rowValue,
                    [Grid.RowProperty] = row,
                    [Grid.ColumnProperty] = 0
                };

                textControl.Classes.Add("SmallLabel");
                grid.Children.Add(textControl);

                foreach (var columnValue in columnValues) {
                    var checkBox = new CheckBox {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        [Grid.RowProperty] = row,
                        [Grid.ColumnProperty] = column
                    };

                    checkBox.Classes.Add("NoText");

                    grid.Children.Add(checkBox);
                    column--;
                }

                columnValues.Remove(rowValue);
                row++;
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}