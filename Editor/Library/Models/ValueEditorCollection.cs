namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Macabresoft.Core;

    public class ValueEditorCollection : IReadOnlyCollection<IValueEditor>, IDisposable {
        private readonly List<IValueEditor> _valueEditors;

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
        public ValueEditorCollection(IEnumerable<IValueEditor> valueEditors, object owner, string name) : this(valueEditors, owner, name, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorCollection" /> class.
        /// </summary>
        /// <param name="valueEditors">The value editors.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="name">The name of the encompassing object being edited.</param>
        /// <param name="deleteCommand">The command to delete this from the editing object..</param>
        public ValueEditorCollection(IEnumerable<IValueEditor> valueEditors, object owner, string name, ICommand deleteCommand) {
            this._valueEditors = valueEditors?.ToList() ?? new List<IValueEditor>();
            this.Owner = owner;
            this.Name = name;
            this.DeleteCommand = deleteCommand;

            foreach (var valueEditor in this.ValueEditors) {
                valueEditor.ValueChanged += this.ValueEditor_ValueChanged;
            }
        }

        /// <inheritdoc />
        public int Count => this.ValueEditors.Count;

        /// <summary>
        /// Gets the command to delete the object being edited from its parent.
        /// </summary>
        public ICommand DeleteCommand { get; }

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

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void ValueEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.OwnedValueChanged.SafeInvoke(sender, e);
        }
    }
}