namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using Macabresoft.Core;
using Macabre2D.Framework;
using Unity;

public partial class ColliderEditor : ValueEditorControl<Collider> {
    public static readonly DirectProperty<ColliderEditor, IReadOnlyCollection<Type>> DerivedTypesProperty =
        AvaloniaProperty.RegisterDirect<ColliderEditor, IReadOnlyCollection<Type>>(
            nameof(DerivedTypes),
            editor => editor.DerivedTypes);

    public static readonly DirectProperty<ColliderEditor, Type> SelectedTypeProperty =
        AvaloniaProperty.RegisterDirect<ColliderEditor, Type>(
            nameof(SelectedType),
            editor => editor.SelectedType,
            (editor, value) => editor.SelectedType = value);

    private readonly HashSet<IValueControl> _childEditors = new();
    private readonly ObservableCollectionExtended<Type> _derivedTypes = new();
    private readonly IValueControlService _valueControlService;

    private ValueControlCollection _controlCollection;
    private Type _selectedType;

    public ColliderEditor() : this(null, Resolver.Resolve<IAssemblyService>(), Resolver.Resolve<IValueControlService>()) {
    }

    [InjectionConstructor]
    public ColliderEditor(
        ValueControlDependencies dependencies,
        IAssemblyService assemblyService,
        IValueControlService valueControlService) : base(dependencies) {
        this._valueControlService = valueControlService;

        var types = assemblyService.LoadTypes(typeof(Collider));
        this._derivedTypes.Reset(types);
        this._derivedTypes.Remove(typeof(PolygonCollider));
        this.ResetColliderEditors();

        this.InitializeComponent();
    }

    public IReadOnlyCollection<Type> DerivedTypes => this._derivedTypes;

    public Type SelectedType {
        get => this._selectedType;
        set {
            if (value != null) {
                this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value);
                Dispatcher.UIThread.Post(() => { this.Value = Activator.CreateInstance(value) as Collider; });
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IValueEditor.Collection)) {
            this.ResetColliderEditors();
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<Collider> args) {
        base.OnValueChanged(args);
        this.ResetColliderEditors();
    }

    private void ClearEditors() {
        if (this._childEditors.Any()) {
            this.Collection.RemoveControls(this._childEditors);
            this._childEditors.Clear();
            this._valueControlService.ReturnControls(this._controlCollection);
            this._controlCollection = null;
        }
    }

    private void ResetColliderEditors() {
        try {
            this.IgnoreUpdates = true;
            this.ClearEditors();

            if (this._valueControlService != null && this.Collection != null && this.Value is { } value) {
                this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value.GetType());
                this._controlCollection = this._valueControlService.CreateControl(value, string.Empty);
                if (this._controlCollection != null) {
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
}