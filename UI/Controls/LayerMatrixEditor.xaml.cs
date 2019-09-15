namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class LayerMatrixEditor : UserControl {
        private readonly Dictionary<Layers, TextBlock> _columnLayerToTextBlock = new Dictionary<Layers, TextBlock>();
        private readonly Dictionary<Layers, CheckBox> _layersToCheckBox = new Dictionary<Layers, CheckBox>();
        private readonly Dictionary<Layers, TextBlock> _rowLayerToTextBlock = new Dictionary<Layers, TextBlock>();
        private IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public LayerMatrixEditor() {
            this.FlipShouldCollideCommand = new RelayCommand<Layers>(this.FlipShouldCollide);
            this.Loaded += this.LayerMatrixEditor_Loaded;
            this.InitializeComponent();
        }

        public ICommand FlipShouldCollideCommand { get; }

        private void FlipShouldCollide(Layers layers) {
            var hasChanges = this._projectService.HasChanges;
            var checkBox = this._layersToCheckBox[layers];
            var originalValue = checkBox.IsChecked;
            var undoCommand = new UndoCommand(() => {
                GameSettings.Instance.Layers.ToggleShouldCollide(layers);
                checkBox.IsChecked = originalValue;
                this._projectService.HasChanges = true;
            }, () => {
                GameSettings.Instance.Layers.ToggleShouldCollide(layers);
                checkBox.IsChecked = !originalValue;
                this._projectService.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
        }

        private void LayerMatrixEditor_Loaded(object sender, RoutedEventArgs e) {
            var layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().OrderBy(x => (byte)x).ToList();
            layers.Remove(Layers.None);
            layers.Remove(Layers.All);
            var gridChildren = this._layersGrid.Children.OfType<CheckBox>().ToList();

            for (var row = -1; row < layers.Count; row++) {
                this._layersGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var columnCount = 0;

                for (var column = layers.Count; column >= row; column--) {
                    if (row < 0) {
                        if (columnCount > 0 && column >= 0) {
                            var textBlock = new TextBlock() {
                                Text = GameSettings.Instance.Layers.GetLayerName(layers[column]),
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                            textBlock.LayoutTransform = new RotateTransform(90);
                            Grid.SetRow(textBlock, row + 1);
                            Grid.SetColumn(textBlock, columnCount);
                            this._layersGrid.Children.Add(textBlock);
                            this._columnLayerToTextBlock[layers[column]] = textBlock;
                        }
                    }
                    else if (columnCount == 0) {
                        var textBlock = new TextBlock() {
                            Text = GameSettings.Instance.Layers.GetLayerName(layers[row]),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right
                        };

                        Grid.SetRow(textBlock, row + 1);
                        Grid.SetColumn(textBlock, columnCount);
                        this._layersGrid.Children.Add(textBlock);
                        this._rowLayerToTextBlock[layers[row]] = textBlock;
                    }
                    else {
                        var rowLayer = layers[row];
                        this._layersGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                        var columnLayer = layers[column];
                        var checkBox = new CheckBox() {
                            Command = this.FlipShouldCollideCommand,
                            CommandParameter = rowLayer | columnLayer,
                            IsChecked = GameSettings.Instance.Layers.GetShouldCollide(rowLayer, columnLayer)
                        };

                        Grid.SetRow(checkBox, row + 1);
                        Grid.SetColumn(checkBox, columnCount);
                        this._layersGrid.Children.Add(checkBox);
                        this._layersToCheckBox[rowLayer | columnLayer] = checkBox;
                    }

                    columnCount++;
                }
            }

            GameSettings.Instance.Layers.LayerNameChanged += this.Layers_LayerNameChanged;

            this.Loaded -= this.LayerMatrixEditor_Loaded;
        }

        private void Layers_LayerNameChanged(object sender, LayerNameChangedEventArgs e) {
            if (this._rowLayerToTextBlock.TryGetValue(e.Layer, out var rowTextBlock) && rowTextBlock != null) {
                rowTextBlock.Text = e.Name;
            }

            if (this._columnLayerToTextBlock.TryGetValue(e.Layer, out var columnTextBlock) && columnTextBlock != null) {
                columnTextBlock.Text = e.Name;
            }
        }
    }
}