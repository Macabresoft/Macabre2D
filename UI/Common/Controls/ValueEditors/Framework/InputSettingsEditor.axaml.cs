namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
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

    private void ConstructControl() {
        if (this.Value == null) {
            return;
        }

        var grid = this.LogicalChildren.OfType<Grid>().First();

        var rowValues = Enum.GetValues<InputAction>().ToList();

        var controllerButtons = Enum.GetValues<Buttons>().ToList();
        var keys = Enum.GetValues<Keys>().ToList();
        var mouseButtons = Enum.GetValues<MouseButton>().ToList();


        for (var i = 0; i <= rowValues.Count; i++) {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }
        
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (var i = 0; i < rowValues.Count; i++) {
            var row = i * 2;
            var action = rowValues[i];

            var control = new InputActionControl(this._undoService, this.Value, action, controllerButtons, keys, mouseButtons) {
                [Grid.RowProperty] = row
            };

            grid.Children.Add(control);

            if (i < rowValues.Count - 1) {
                var separator = new Border {
                    [Grid.RowProperty] = row + 1,
                    [Grid.ColumnProperty] = 0,
                    [Grid.ColumnSpanProperty] = 2
                };

                separator.Classes.Add("HorizontalSeparator");
                grid.Children.Add(separator);
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}