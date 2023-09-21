namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
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

    public IReadOnlyCollection<Type> AvailableTypes { get; } = new[] {
        typeof(bool)
    };

    public ICommand RemoveCommand { get; }

    public IReadOnlyCollection<NamedSettingModel> Settings => this._settings;

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<NamedSettingCollection> args) {
        base.OnValueChanged(args);

        if (this.Value != null) {
            this._settings.Reset(this.Value.Select(x => new NamedSettingModel(x, this.Value, this._undoService)));
        }
    }

    private void Add() {
        var newModel = new NamedSettingModel(new BoolSetting(), this.Value, this._undoService);

        this._undoService.Do(() =>
        {
            this._settings.Add(newModel);
            this.Value.AddSetting(newModel.Setting);
        }, () =>
        {
            this._settings.Remove(newModel);
            this.Value.RemoveSetting(newModel.Setting);
        });
    }

    private void Remove(NamedSettingModel model) {
        if (model == null) {
            return;
        }

        this._undoService.Do(() =>
        {
            this._settings.Remove(model);
            this.Value.RemoveSetting(model.Setting);
        }, () =>
        {
            this._settings.Add(model);
            this.Value.AddSetting(model.Setting);
        });
    }
}