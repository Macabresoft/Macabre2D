namespace Macabresoft.Macabre2D.UI.Common.Models {
    using System;
    using Macabresoft.Core;

    /// <summary>
    /// A command for undoing/redoing changes.
    /// </summary>
    public sealed class UndoCommand {
        private readonly Action _action;
        private readonly Action _propertyChangedAction;
        private readonly Action _undoAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCommand" /> class.
        /// </summary>
        /// <param name="action">The action that can be undone.</param>
        /// <param name="undoAction">The action that undoes the changes performed in <see cref="_action" />.</param>
        /// <param name="scope">The scope of the undo operation.</param>
        /// <exception cref="ArgumentException">An action being passed should not be null.</exception>
        public UndoCommand(Action action, Action undoAction, UndoScope scope) {
            this._action = action ?? throw new ArgumentException(nameof(action));
            this._undoAction = undoAction ?? throw new ArgumentException(nameof(undoAction));
            this.Scope = scope;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCommand" /> class.
        /// </summary>
        /// <param name="action">The action that can be undone.</param>
        /// <param name="undoAction">The action that undoes the changes performed in <see cref="_action" />.</param>
        /// <param name="scope">The scope of the undo operation.</param>
        /// <param name="propertyChangedAction">An action which will kick off an object's property changed notification.</param>
        public UndoCommand(Action action, Action undoAction, UndoScope scope, Action propertyChangedAction) : this(action, undoAction, scope) {
            this._propertyChangedAction = propertyChangedAction;
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public UndoScope Scope { get; }

        /// <summary>
        /// Performs the operation.
        /// </summary>
        public void Do() {
            this._action.SafeInvoke();
            this._propertyChangedAction.SafeInvoke();
        }

        /// <summary>
        /// Performs the undo operation.
        /// </summary>
        public void Undo() {
            this._undoAction.SafeInvoke();
            this._propertyChangedAction.SafeInvoke();
        }
    }
}