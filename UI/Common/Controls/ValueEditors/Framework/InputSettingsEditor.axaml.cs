namespace Macabresoft.Macabre2D.UI.Common.Controls.ValueEditors.Framework;

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

public class InputSettingsEditor : ValueEditorControl<InputSettings> {
    private readonly IUndoService _undoService;

    public InputSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public InputSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
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

    private void ConstructControl() {
        if (this.Value == null) {
            return;
        }

        var grid = this.LogicalChildren.OfType<Grid>().First();

        var rowValues = Enum.GetValues<InputAction>().ToList();

        for (var i = 0; i <= rowValues.Count + 1; i++) {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (var i = 0; i < rowValues.Count; i++) {
            var row = i * 2;
            var action = rowValues[row];
            var label = new TextBlock {
                Text = action.ToString(),
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 0
            };

            label.Classes.Add("Label");
            grid.Children.Add(label);

            var textBox = new TextBox {
                Text = this.Value.GetName(action),
                Tag = action,
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 1
            };

            textBox.LostFocus += this.LayerTextBox_LostFocus;
            textBox.KeyUp += this.LayerTextBox_KeyUp;
            grid.Children.Add(textBox);

            var separator = new Border() {
                [Grid.RowProperty] = row + 1,
                [Grid.ColumnProperty] = 0,
                [Grid.ColumnSpanProperty] = 2
            };
            
            separator.Classes.Add("HorizontalSeparator");
            grid.Children.Add(separator);
        }
    }

    private void HandleTextBoxKey(TextBox textBox, InputAction action, KeyEventArgs e) {
        if (this.Value == null) {
            return;
        }

        if (e.Key == Key.Enter) {
            this.TryUpdateLayerName(textBox, action);
        }
        else if (e.Key == Key.Escape) {
            var originalName = this.Value.GetName(action);
            textBox.Text = originalName;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void LayerTextBox_KeyUp(object sender, KeyEventArgs e) {
        if (this.Value != null && sender is TextBox { Tag: InputAction action } textBox) {
            this.HandleTextBoxKey(textBox, action, e);
        }
    }

    private void LayerTextBox_LostFocus(object sender, RoutedEventArgs e) {
        if (this.Value != null && sender is TextBox { Tag: InputAction action } textBox) {
            this.TryUpdateLayerName(textBox, action);
        }
    }
    
    private void TryUpdateLayerName(TextBox textBox, InputAction action) {
        if (this.Value == null) {
            return;
        }

        var originalName = this.Value.GetName(action);
        var updatedName = textBox.Text;

        if (originalName != updatedName && this.Value.SetName(action, updatedName)) {
            this._undoService.Do(() =>
            {
                this.Value.SetName(action, updatedName);
                textBox.Text = updatedName;
            }, () =>
            {
                this.Value.SetName(action, originalName);
                textBox.Text = originalName;
            });
        }
    }

    private void UnsubscribeFromEvents() {
        foreach (var textBox in this.LogicalChildren.OfType<TextBox>()) {
            textBox.LostFocus -= this.LayerTextBox_LostFocus;
            textBox.KeyUp -= this.LayerTextBox_KeyUp;
        }
    }
}