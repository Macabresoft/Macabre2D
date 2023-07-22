namespace Macabresoft.Macabre2D.UI.Editor;

using System.ComponentModel;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Microsoft.Xna.Framework;
using Unity;

public partial class SingleColorShaderReferenceEditor : ValueEditorControl<SingleColorShaderReference> {
    private readonly IUndoService _undoService;
    private bool _isEditing;
    private string _parameterName = string.Empty;
    private Color _parameterValue = Color.Transparent;

    public SingleColorShaderReferenceEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SingleColorShaderReferenceEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<SingleColorShaderReference> args) {
        base.OnValueChanged(args);

        if (args.OldValue is { HasValue: true, Value: { } reference }) {
            reference.PropertyChanged -= this.Value_PropertyChanged;
        }

        if (this.Value != null) {
            this.Value.PropertyChanged += this.Value_PropertyChanged;
            this._parameterName = this.Value.ParameterName;
            this._parameterValue = this.Value.ParameterValue;
        }
    }

    private void SetParameterName(string name) {
        try {
            this._isEditing = true;
            this.Value.ParameterName = name;
            this._parameterName = name;
        }
        finally {
            this._isEditing = false;
        }
    }

    private void SetParameterValue(Color color) {
        try {
            this._isEditing = true;
            this.Value.ParameterValue = color;
            this._parameterValue = color;
        }
        finally {
            this._isEditing = false;
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (!this._isEditing && sender is SingleColorShaderReference shaderReference) {
            if (e.PropertyName is nameof(SingleColorShaderReference.ParameterValue)) {
                var previous = this._parameterValue;
                var current = shaderReference.ParameterValue;

                if (previous != current) {
                    this._undoService.Do(() => { this.SetParameterValue(current); }, () => { this.SetParameterValue(previous); });
                }
            }
            else if (e.PropertyName is nameof(SingleColorShaderReference.ParameterName)) {
                var previous = this._parameterName;
                var current = shaderReference.ParameterName;

                if (previous != current) {
                    this._undoService.Do(() => { this.SetParameterName(current); }, () => { this.SetParameterName(previous); });
                }
            }
        }
    }
}