namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public interface IUndoService : INotifyPropertyChanged {
        bool CanRedo { get; }

        bool CanUndo { get; }

        void Clear();

        void Do(UndoCommand undoCommand);

        void Redo();

        void Undo();
    }

    public sealed class UndoService : NotifyPropertyChanged, IUndoService {
        private readonly Stack<UndoCommand> _redoStack = new Stack<UndoCommand>();
        private readonly Stack<UndoCommand> _undoStack = new Stack<UndoCommand>();

        public bool CanRedo {
            get {
                return this._redoStack.Any();
            }
        }

        public bool CanUndo {
            get {
                return this._undoStack.Any();
            }
        }

        public void Clear() {
            this._redoStack.Clear();
            this._undoStack.Clear();
            this.RaiseProperties();
        }

        public void Do(UndoCommand undoCommand) {
            undoCommand.Do();
            this._undoStack.Push(undoCommand);
            this._redoStack.Clear();
            this.RaiseProperties();
        }

        public void Redo() {
            if (this.CanRedo) {
                var command = this._redoStack.Pop();
                command.Do();
                this._undoStack.Push(command);
                this.RaiseProperties();
            }
        }

        public void Undo() {
            if (this.CanUndo) {
                var command = this._undoStack.Pop();
                command.Undo();
                this._redoStack.Push(command);
                this.RaiseProperties();
            }
        }

        private void RaiseProperties() {
            this.RaisePropertyChanged(nameof(this.CanRedo));
            this.RaisePropertyChanged(nameof(this.CanUndo));
        }
    }
}