namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
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
        this.CreateMap();
    }

    private void CreateMap() {
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

            var textBox = new TextBox {
                Text = this.Value.GetName(layer),
                Tag = layer,
                IsEnabled = false,
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 1
            };
            
            // subscribe to property changed on text, using tag to do updates
            
            grid.Children.Add(textBox);
        }
        
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}