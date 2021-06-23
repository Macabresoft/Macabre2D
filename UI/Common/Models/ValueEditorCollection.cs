namespace Macabresoft.Macabre2D.UI.Common.Models {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Macabresoft.Core;using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A collection of value editors.
    /// </summary>
    public class ValueEditorCollection : IReadOnlyCollection<IValueEditor>, IDisposable {
        private readonly ObservableCollection<IValueEditor> _valueEditors = new();

        /// <summary>
        /// Raised when a value editor owned by this collection has its value change.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<object>> OwnedValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorCollection" /> class.
        /// </summary>
        /// <param name="valueEditors">The value editors.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="name">The name of the encompassing object being edited.</param>
        public ValueEditorCollection(IEnumerable<IValueEditor> valueEditors, object owner, string name) {
            this.Owner = owner;
            this.Name = name;

            this.AddEditors(valueEditors);
        }

        /// <inheritdoc />
        public int Count => this.ValueEditors.Count;

        /// <summary>
        /// Gets the name of the object being edited.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        public object Owner { get; }

        /// <summary>
        /// Gets the value editors for the object being edited.
        /// </summary>
        public IReadOnlyCollection<IValueEditor> ValueEditors => this._valueEditors;

        /// <summary>
        /// Adds the editors.
        /// </summary>
        /// <param name="valueEditors">The editors to add.</param>
        public void AddEditors(IEnumerable<IValueEditor> valueEditors) {
            if (valueEditors != null) {
                foreach (var valueEditor in valueEditors) {
                    valueEditor.ValueChanged += this.ValueEditor_ValueChanged;
                    valueEditor.Collection = this;
                    this._valueEditors.Add(valueEditor);
                }
            }
        }

        /// <inheritdoc />
        public void Dispose() {
            foreach (var valueEditor in this.ValueEditors) {
                valueEditor.ValueChanged -= this.ValueEditor_ValueChanged;
            }

            this._valueEditors.Clear();
            this.OwnedValueChanged = null;
        }

        /// <inheritdoc />
        public IEnumerator<IValueEditor> GetEnumerator() {
            return this.ValueEditors.GetEnumerator();
        }

        /// <summary>
        /// Removes the editors.
        /// </summary>
        /// <param name="valueEditors">The editors to remove.</param>
        public void RemoveEditors(IEnumerable<IValueEditor> valueEditors) {
            if (valueEditors != null) {
                foreach (var valueEditor in valueEditors) {
                    valueEditor.ValueChanged -= this.ValueEditor_ValueChanged;
                    this._valueEditors.Remove(valueEditor);
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void ValueEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.OwnedValueChanged.SafeInvoke(sender, e);
        }
    }
}