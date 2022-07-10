namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Unity;

public class RelativeVerticesEditor : ValueEditorControl<RelativeVertices> {
    public static readonly DirectProperty<RelativeVerticesEditor, IReadOnlyCollection<NotifyingWrapper<Vector2>>> WrappedValuesProperty =
        AvaloniaProperty.RegisterDirect<RelativeVerticesEditor, IReadOnlyCollection<NotifyingWrapper<Vector2>>>(
            nameof(WrappedValues),
            editor => editor.WrappedValues);

    private readonly IUndoService _undoService;

    private readonly ObservableCollectionExtended<NotifyingWrapper<Vector2>> _wrappedValues = new();
    private bool _isUpdatingFromWrappedValue;

    public RelativeVerticesEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RelativeVerticesEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    public IReadOnlyCollection<NotifyingWrapper<Vector2>> WrappedValues => this._wrappedValues;

    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (!this._isUpdatingFromWrappedValue && this.Value != null) {
            foreach (var value in this._wrappedValues) {
                value.PropertyChanged -= this.WrappedValue_PropertyChanged;
            }

            this._wrappedValues.Reset(this.Value.Select(x => new NotifyingWrapper<Vector2>(x)));

            foreach (var value in this._wrappedValues) {
                value.PropertyChanged += this.WrappedValue_PropertyChanged;
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateValue(NotifyingWrapper<Vector2> wrapper, Vector2 value, int index) {
        try {
            this._isUpdatingFromWrappedValue = true;
            wrapper.Value = value;
            this.Value.Replace(value, index);
        }
        finally {
            this._isUpdatingFromWrappedValue = false;
        }
    }

    private void WrappedValue_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (sender is NotifyingWrapper<Vector2> wrapper && this.Value != null && !this._isUpdatingFromWrappedValue) {
            var index = this._wrappedValues.IndexOf(wrapper);
            var newValue = wrapper.Value;
            var originalValue = this.Value[index];

            this._undoService.Do(() => { this.UpdateValue(wrapper, newValue, index); }, () => { this.UpdateValue(wrapper, originalValue, index); });
        }
    }
}