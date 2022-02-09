namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class LayerSettingsEditor : ValueEditorControl<LayerSettings> {
    private readonly IUndoService _undoService;

    public LayerSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public LayerSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.ConstructControl();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnDetachedFromLogicalTree(e);
        this.UnsubscribeFromEvents();
    }

    private void CheckBox_Toggled(object sender, RoutedEventArgs e) {
        if (this.Value != null && sender is CheckBox { Tag: Layers layer } checkBox) {
            this.ToggleLayer(checkBox, layer);
        }
    }

    private void ConstructControl() {
        if (this.Value == null) {
            return;
        }

        var grid = this.LogicalChildren.OfType<Grid>().First();

        var rowValues = Enum.GetValues<Layers>().ToList();
        rowValues.Remove(Layers.None);

        for (var i = 0; i <= rowValues.Count + 1; i++) {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }
        
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (var row = 0; row < rowValues.Count; row++) {
            var layer = rowValues[row];
            var label = new TextBlock {
                Text = layer.ToString(),
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 0
            };

            label.Classes.Add("Label");
            grid.Children.Add(label);
            
            var checkBox = new CheckBox {
                IsChecked = this.Value.IsLayerEnabled(layer),
                IsEnabled = layer != Layers.Default,
                Tag = layer,
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 1
            };

            checkBox.Checked += this.CheckBox_Toggled;
            checkBox.Unchecked += this.CheckBox_Toggled;

            grid.Children.Add(checkBox);

            var textBox = new TextBox {
                Text = this.Value.GetName(layer),
                Tag = layer,
                IsReadOnly = layer == Layers.Default,
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 2
            };

            textBox.LostFocus += this.LayerTextBox_LostFocus;
            textBox.KeyUp += this.LayerTextBox_KeyUp;
            grid.Children.Add(textBox);
        }
    }

    private void HandleTextBoxKey(TextBox textBox, Layers layer, KeyEventArgs e) {
        if (this.Value == null) {
            return;
        }

        if (e.Key == Key.Enter) {
            this.TryUpdateLayerName(textBox, layer);
        }
        else if (e.Key == Key.Escape) {
            var originalName = this.Value.GetName(layer);
            textBox.Text = originalName;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void LayerTextBox_KeyUp(object sender, KeyEventArgs e) {
        if (this.Value != null && sender is TextBox { Tag: Layers layer } textBox) {
            this.HandleTextBoxKey(textBox, layer, e);
        }
    }

    private void LayerTextBox_LostFocus(object sender, RoutedEventArgs e) {
        if (this.Value != null && sender is TextBox { Tag: Layers layer } textBox) {
            this.TryUpdateLayerName(textBox, layer);
        }
    }

    private void ToggleLayer(CheckBox checkBox, Layers layer) {
        if (this.Value == null) {
            return;
        }

        if (checkBox.IsChecked == true) {
            if (!this.Value.IsLayerEnabled(layer)) {
                this._undoService.Do(() => {
                    this.Value.EnableLayers(layer);
                    checkBox.IsChecked = this.Value.IsLayerEnabled(layer);
                }, () => {
                    this.Value.DisableLayers(layer);
                    checkBox.IsChecked = this.Value.IsLayerEnabled(layer);
                });
            }
        }
        else {
            if (this.Value.IsLayerEnabled(layer)) {
                this._undoService.Do(() => {
                    this.Value.DisableLayers(layer);
                    checkBox.IsChecked = this.Value.IsLayerEnabled(layer);
                }, () => {
                    this.Value.EnableLayers(layer);
                    checkBox.IsChecked = this.Value.IsLayerEnabled(layer);
                });
            }
        }
    }

    private void TryUpdateLayerName(TextBox textBox, Layers layer) {
        if (this.Value == null) {
            return;
        }

        var originalName = this.Value.GetName(layer);
        var updatedName = textBox.Text;

        if (originalName != updatedName && this.Value.SetName(layer, updatedName)) {
            this._undoService.Do(() => {
                this.Value.SetName(layer, updatedName);
                textBox.Text = updatedName;
            }, () => {
                this.Value.SetName(layer, originalName);
                textBox.Text = originalName;
            });
        }
    }

    private void UnsubscribeFromEvents() {
        foreach (var textBox in this.LogicalChildren.OfType<TextBox>()) {
            textBox.LostFocus -= this.LayerTextBox_LostFocus;
            textBox.KeyUp -= this.LayerTextBox_KeyUp;
        }

        foreach (var checkBox in this.LogicalChildren.OfType<CheckBox>()) {
            checkBox.Checked -= this.CheckBox_Toggled;
            checkBox.Unchecked -= this.CheckBox_Toggled;
        }
    }
}