namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

public class LayerSettingsEditor : ValueEditorControl<LayerSettings> {
    public LayerSettingsEditor() : this(null) {
    }

    [InjectionConstructor]
    public LayerSettingsEditor(ValueControlDependencies dependencies) : base(dependencies) {
        var layers = Enum.GetValues<Layers>().ToList();
        layers.Remove(Layers.None);
        this.AvailableLayers = layers;

        this.InitializeComponent();
    }

    public IReadOnlyCollection<Layers> AvailableLayers { get; }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}