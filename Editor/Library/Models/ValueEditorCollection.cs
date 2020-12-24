namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    public class ValueEditorCollection : IReadOnlyCollection<IValueEditor> {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorCollection" /> class.
        /// </summary>
        /// <param name="valueEditors">The value editors.</param>
        /// <param name="name">The name of the encompassing object being edited.</param>
        public ValueEditorCollection(IEnumerable<IValueEditor> valueEditors, string name) : this(valueEditors, name, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorCollection" /> class.
        /// </summary>
        /// <param name="valueEditors">The value editors.</param>
        /// <param name="name">The name of the encompassing object being edited.</param>
        /// <param name="deleteCommand">The command to delete this from the editing object..</param>
        public ValueEditorCollection(IEnumerable<IValueEditor> valueEditors, string name, ICommand deleteCommand) {
            this.ValueEditors = valueEditors?.ToList() ?? new List<IValueEditor>();
            this.Name = name;
            this.DeleteCommand = deleteCommand;
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
        /// Gets the value editors for the object being edited.
        /// </summary>
        public IReadOnlyCollection<IValueEditor> ValueEditors { get; }

        /// <inheritdoc />
        public IEnumerator<IValueEditor> GetEnumerator() {
            return this.ValueEditors.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}