namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using Macabre2D.Framework;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Unity;

public partial class ShaderAssetEditor : ValueEditorControl<ShaderAsset> {
    public static readonly DirectProperty<ShaderAssetEditor, IReadOnlyCollection<Type>> DerivedTypesProperty =
        AvaloniaProperty.RegisterDirect<ShaderAssetEditor, IReadOnlyCollection<Type>>(
            nameof(DerivedTypes),
            editor => editor.DerivedTypes);

    public static readonly DirectProperty<ShaderAssetEditor, Type> SelectedTypeProperty =
        AvaloniaProperty.RegisterDirect<ShaderAssetEditor, Type>(
            nameof(SelectedType),
            editor => editor.SelectedType,
            (editor, value) => editor.SelectedType = value);

    private readonly HashSet<IValueControl> _childEditors = [];
    private readonly ObservableCollectionExtended<Type> _derivedTypes = [];
    private readonly IUndoService _undoService;
    private readonly IValueControlService _valueControlService;

    private ValueControlCollection _controlCollection;

    public ShaderAssetEditor() : this(
        null,
        Resolver.Resolve<IAssemblyService>(),
        Resolver.Resolve<IValueControlService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ShaderAssetEditor(
        ValueControlDependencies dependencies,
        IAssemblyService assemblyService,
        IValueControlService valueControlService,
        IUndoService undoService) : base(dependencies) {
        this._valueControlService = valueControlService;
        this._undoService = undoService;

        var types = assemblyService.LoadTypes(typeof(IShaderConfiguration));
        this._derivedTypes.Reset(types);
        this.ResetShaderEditors();

        this.InitializeComponent();
    }

    public IReadOnlyCollection<Type> DerivedTypes => this._derivedTypes;

    public Type SelectedType {
        get => this.Value.ConfigurationType;
        set {
            var originalValue = this.SelectedType;

            if (value != null && value != originalValue) {
                this._undoService.Do(
                    () => this.SetSelectedType(originalValue, value),
                    () => this.SetSelectedType(value, originalValue));
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IValueEditor.Collection)) {
            this.ResetShaderEditors();
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<ShaderAsset> args) {
        base.OnValueChanged(args);
        this.ResetShaderEditors();
    }

    private void ClearEditors() {
        if (this._childEditors.Any()) {
            this.Collection.RemoveControls(this._childEditors);
            this._childEditors.Clear();
            this._valueControlService.ReturnControls(this._controlCollection);
            this._controlCollection = null;
        }
    }

    private void ResetShaderEditors() {
        try {
            this.IgnoreUpdates = true;
            this.ClearEditors();

            if (this._valueControlService != null && this.Collection != null && this.Value is { } value) {
                this._controlCollection = this._valueControlService.CreateControl(value, string.Empty, false);
                if (this._controlCollection != null) {
                    if (this._controlCollection.ValueControls.FirstOrDefault(x => x.ValuePropertyName == nameof(ShaderAsset.ConfigurationType)) is { } control) {
                        this._controlCollection.RemoveControls([control]);
                    }

                    this._childEditors.Clear();
                    this._childEditors.AddRange(this._controlCollection.ValueControls);
                    this.Collection.AddControls(this._childEditors);
                }
            }
        }
        finally {
            this.IgnoreUpdates = false;
        }
    }

    private void SetSelectedType(Type originalValue, Type updatedValue) {
        this.Value.ConfigurationType = updatedValue;
        Dispatcher.UIThread.Post(() => { this.RaisePropertyChanged(SelectedTypeProperty, originalValue, updatedValue); });

        this.ResetShaderEditors();
    }
}