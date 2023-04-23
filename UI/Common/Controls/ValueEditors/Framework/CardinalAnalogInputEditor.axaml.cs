namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class CardinalAnalogInputEditor : ValueEditorControl<CardinalAnalogInput> {
    public CardinalAnalogInputEditor() : this(null) {
    }

    [InjectionConstructor]
    public CardinalAnalogInputEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}