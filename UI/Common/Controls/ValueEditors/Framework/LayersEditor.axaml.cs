namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public class LayersEditor : ValueEditorControl<Layers> {
    public static readonly DirectProperty<FlagsEnumEditor, ICommand> SelectAllCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(SelectAllCommand),
            editor => editor.SelectAllCommand);

    public static readonly DirectProperty<FlagsEnumEditor, ICommand> ToggleValueCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(ToggleValueCommand),
            editor => editor.ToggleValueCommand);

    private readonly IProjectService _projectService;

    public LayersEditor() : this(null, Resolver.Resolve<IProjectService>()) {
    }

    [InjectionConstructor]
    public LayersEditor(ValueControlDependencies dependencies, IProjectService projectService) : base(dependencies) {
        this._projectService = projectService;
        this.EnabledLayers = this.GetEnabledLayers();
        this.ClearCommand = ReactiveCommand.Create(this.Clear);
        this.SelectAllCommand = ReactiveCommand.Create(this.SelectAll);
        this.ToggleValueCommand = ReactiveCommand.Create<Layers>(this.ToggleValue);
        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public IReadOnlyCollection<Layers> EnabledLayers { get; }

    public ICommand SelectAllCommand { get; }

    public ICommand ToggleValueCommand { get; }

    private void Clear() {
        this.Value = Layers.None;
    }

    private IReadOnlyCollection<Layers> GetEnabledLayers() {
        return Enum.GetValues<Layers>()
            .Where(x => (this._projectService.CurrentProject.Settings.LayerSettings.EnabledLayers & x) != Layers.None)
            .ToList();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectAll() {
        var all = this._projectService.CurrentProject.Settings.LayerSettings.EnabledLayers;
        if (all != this.Value) {
            this.Value = all;
        }
    }

    private void SelectingItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (sender is ComboBox comboBox) {
            comboBox.SelectedItem = null;
        }
    }

    private void ToggleValue(Layers toggled) {
        if (this.Value.HasFlag(toggled)) {
            this.Value &= ~toggled;
        }
        else {
            this.Value |= toggled;
        }
    }
}