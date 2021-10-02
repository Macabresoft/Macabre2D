namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using System;
    using System.Globalization;
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Avalonia.VisualTree;
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
            var grid = this.LogicalChildren.OfType<Grid>().First();
            var rowValues = Enum.GetValues<Layers>().ToList();
            rowValues.Remove(Layers.Default);
            rowValues.Remove(Layers.None);
            var columnValues = rowValues.ToList();

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            var row = 1;
            var column = 1;

            foreach (var columnValue in columnValues) {
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                var layoutTransform = new LayoutTransformControl {
                    ClipToBounds = false,
                    LayoutTransform = new RotateTransform(90),
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = column
                };
                
                var textControl = new TextBlock() {
                    Text = this._displayNameConverter.Convert(columnValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                layoutTransform.Child = textControl;
                
                grid.Children.Add(layoutTransform);
                column++;
            }

            foreach (var rowValue in rowValues) {
                column = 1;
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                var textControl = new TextBlock {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = this._displayNameConverter.Convert(rowValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                    [Grid.RowProperty] = row,
                    [Grid.ColumnProperty] = 0
                };
                
                grid.Children.Add(textControl);

                foreach (var columnValue in columnValues) {
                    var checkBox = new CheckBox() {
                        [Grid.RowProperty] = row,
                        [Grid.ColumnProperty] = column
                    };
                    
                    grid.Children.Add(checkBox);
                    column++;
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