namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using DynamicData.Binding;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class NamedSettingCollectionEditor : ValueEditorControl<NamedSettingCollection> {
    public static readonly DirectProperty<NamedSettingCollectionEditor, ICommand> AddCommandProperty =
        AvaloniaProperty.RegisterDirect<NamedSettingCollectionEditor, ICommand>(
            nameof(AddCommand),
            editor => editor.AddCommand);

    public static readonly DirectProperty<NamedSettingCollectionEditor, ICommand> RemoveCommandProperty =
        AvaloniaProperty.RegisterDirect<NamedSettingCollectionEditor, ICommand>(
            nameof(RemoveCommand),
            editor => editor.RemoveCommand);

    public static readonly DirectProperty<NamedSettingCollectionEditor, IReadOnlyCollection<NamedSettingModel>> SettingsProperty =
        AvaloniaProperty.RegisterDirect<NamedSettingCollectionEditor, IReadOnlyCollection<NamedSettingModel>>(
            nameof(Settings),
            editor => editor.Settings);

    private readonly ObservableCollectionExtended<NamedSettingModel> _settings = new();
    private readonly IUndoService _undoService;

    public NamedSettingCollectionEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public NamedSettingCollectionEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.AddCommand = ReactiveCommand.Create(this.Add);
        this.RemoveCommand = ReactiveCommand.Create<NamedSettingModel>(this.Remove);

        this.InitializeComponent();
    }

    public ICommand AddCommand { get; }

    public IReadOnlyCollection<Type> AvailableType { get; } = new[] {
        typeof(bool),
        typeof(string),
        typeof(float),
        typeof(int)
    };

    public ICommand RemoveCommand { get; }

    public IReadOnlyCollection<NamedSettingModel> Settings => this._settings;

    private void Add() {
        this._undoService.Do(() => { }, () => { });
    }

    private void Remove(NamedSettingModel model) {
        if (model == null) {
            return;
        }

        this._undoService.Do(() => { }, () => { });
    }
}