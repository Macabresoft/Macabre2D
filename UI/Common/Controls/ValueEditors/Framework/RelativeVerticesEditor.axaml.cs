namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using ReactiveUI;
using Unity;

public class RelativeVerticesEditor : ValueEditorControl<RelativeVertices> {
    public static readonly DirectProperty<RelativeVerticesEditor, ICommand> AddCommandProperty =
        AvaloniaProperty.RegisterDirect<RelativeVerticesEditor, ICommand>(
            nameof(AddCommand),
            editor => editor.AddCommand);

    public static readonly DirectProperty<RelativeVerticesEditor, ICommand> RemoveCommandProperty =
        AvaloniaProperty.RegisterDirect<RelativeVerticesEditor, ICommand>(
            nameof(RemoveCommand),
            editor => editor.RemoveCommand);

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

        this.AddCommand = ReactiveCommand.Create(this.Add);
        this.RemoveCommand = ReactiveCommand.Create<NotifyingWrapper<Vector2>>(this.Remove);

        this.InitializeComponent();
    }

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

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

    private void Add() {
        try {
            this._isUpdatingFromWrappedValue = true;
            var index = this._wrappedValues.Count;
            var wrapper = new NotifyingWrapper<Vector2>(Vector2.Zero);

            this._undoService.Do(() =>
            {
                this._wrappedValues.Add(wrapper);
                this.Value.Add(Vector2.Zero);
                wrapper.PropertyChanged += this.WrappedValue_PropertyChanged;
            }, () =>
            {
                this._wrappedValues.RemoveAt(index);
                this.Value.RemoveAt(index);
                wrapper.PropertyChanged -= this.WrappedValue_PropertyChanged;
            });
        }
        finally {
            this._isUpdatingFromWrappedValue = false;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void Remove(NotifyingWrapper<Vector2> wrapper) {
        if (wrapper == null) {
            return;
        }

        try {
            this._isUpdatingFromWrappedValue = true;
            var index = this._wrappedValues.IndexOf(wrapper);

            this._undoService.Do(() =>
            {
                this._wrappedValues.RemoveAt(index);
                this.Value.RemoveAt(index);
            }, () =>
            {
                this._wrappedValues.Insert(index, wrapper);
                this.Value.Insert(index, wrapper.Value);
            });
        }
        finally {
            this._isUpdatingFromWrappedValue = false;
        }
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