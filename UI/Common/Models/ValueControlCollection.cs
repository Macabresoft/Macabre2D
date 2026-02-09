namespace Macabre2D.UI.Common;

using Macabresoft.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// A collection of value controls.
/// </summary>
public class ValueControlCollection : PropertyChangedNotifier, IReadOnlyCollection<IValueControl>, IDisposable {
    private readonly ObservableCollection<IValueControl> _valueControls = new();
    private bool _isExpanded = true;

    /// <summary>
    /// Raised when a value editor owned by this collection has its value change.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<object>> OwnedValueChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueControlCollection" /> class.
    /// </summary>
    /// <param name="valueEditors">The value editors.</param>
    /// <param name="name">The name of the encompassing object being edited.</param>
    public ValueControlCollection(IEnumerable<IValueControl> valueEditors, string name) {
        this.Name = name;
        this.AddControls(valueEditors);
    }

    /// <inheritdoc />
    public int Count => this.ValueControls.Count;

    /// <summary>
    /// Gets the name of the object being edited.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the controls in this collection.
    /// </summary>
    public IReadOnlyCollection<IValueControl> ValueControls => this._valueControls;

    /// <summary>
    /// Gets or sets a value indicating whether or not this should be expanded in the UI.
    /// </summary>
    public bool IsExpanded {
        get => this._isExpanded;
        set => this.Set(ref this._isExpanded, value);
    }

    /// <summary>
    /// Adds the controls.
    /// </summary>
    /// <param name="valueControls">The controls to add.</param>
    public void AddControls(IEnumerable<IValueControl> valueControls) {
        if (valueControls != null) {
            foreach (var valueControl in valueControls) {
                if (valueControl is IValueEditor editor) {
                    editor.ValueChanged += this.ValueEditor_ValueChanged;
                }

                this._valueControls.Add(valueControl);
                valueControl.Collection = this;
            }
        }
    }

    /// <inheritdoc />
    public IEnumerator<IValueControl> GetEnumerator() {
        return this.ValueControls.GetEnumerator();
    }

    /// <summary>
    /// Removes the controls.
    /// </summary>
    /// <param name="valueControls">The editors to remove.</param>
    public void RemoveControls(IEnumerable<IValueControl> valueControls) {
        if (valueControls != null) {
            foreach (var valueControl in valueControls) {
                if (valueControl is IValueEditor editor) {
                    editor.ValueChanged -= this.ValueEditor_ValueChanged;
                }

                this._valueControls.Remove(valueControl);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();

        foreach (var valueControl in this.ValueControls) {
            if (valueControl is IValueEditor editor) {
                editor.ValueChanged -= this.ValueEditor_ValueChanged;
            }

            valueControl.Collection = null;
        }

        this._valueControls.Clear();
        this.OwnedValueChanged = null;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    private void ValueEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
        this.OwnedValueChanged.SafeInvoke(sender, e);
    }
}