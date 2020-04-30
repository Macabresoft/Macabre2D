namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.Framework;
    using System;

    public class UndoCommand {
        private readonly Action _action;
        private readonly Action _propertyChangedAction;
        private readonly Action _undoAction;

        public UndoCommand(Action action, Action undoAction) {
            if (action == null) {
                throw new ArgumentException(nameof(action));
            }
            else if (undoAction == null) {
                throw new ArgumentException(nameof(undoAction));
            }

            this._action = action;
            this._undoAction = undoAction;
        }

        public UndoCommand(Action action, Action undoAction, Action propertyChangedAction) : this(action, undoAction) {
            this._propertyChangedAction = propertyChangedAction;
        }

        public virtual void Do() {
            this._action.SafeInvoke();
            this._propertyChangedAction.SafeInvoke();
        }

        public virtual void Undo() {
            this._undoAction.SafeInvoke();
            this._propertyChangedAction.SafeInvoke();
        }
    }
}