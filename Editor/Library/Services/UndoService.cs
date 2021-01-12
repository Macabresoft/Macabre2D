namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which handles undo/redo operations.
    /// </summary>
    public interface IUndoService {
        /// <summary>
        /// Gets a value indicating whether or not a redo operation is possible.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Gets a value indicating whether or not an undo operation is possible.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Clears the queue of undo and redo operations.
        /// </summary>
        void Clear();

        /// <summary>
        /// Performs the specified action and makes it available to be undone. When undoing or redoing the action, the property
        /// changed action will also be called.
        /// </summary>
        /// <param name="action">The action that can be undone.</param>
        /// <param name="undoAction">The action that undoes the changes performed in <see cref="action" />.</param>
        /// <param name="propertyChangedAction">An action which will kick off an object's property changed notification.</param>
        void Do(Action action, Action undoAction, Action propertyChangedAction);

        /// <summary>
        /// Performs the specified action and makes it available to be undone.
        /// </summary>
        /// <param name="action">The action that can be undone.</param>
        /// <param name="undoAction">The action that undoes the changes performed in <see cref="action" />.</param>
        void Do(Action action, Action undoAction);

        /// <summary>
        /// Performs the most recently undone operation.
        /// </summary>
        void Redo();

        /// <summary>
        /// Undoes the most recently performed operation.
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// A service which handles undo/redo operations.
    /// </summary>
    public class UndoService : ReactiveObject, IUndoService {
        private readonly Stack<UndoCommand> _redoStack = new();
        private readonly Stack<UndoCommand> _undoStack = new();
        private readonly object _lock = new();

        /// <inheritdoc />
        public bool CanRedo => this._redoStack.Any();

        /// <inheritdoc />
        public bool CanUndo => this._undoStack.Any();

        /// <inheritdoc />
        public void Clear() {
            this._redoStack.Clear();
            this._undoStack.Clear();
            this.RaiseProperties();
        }

        /// <inheritdoc />
        public void Do(Action action, Action undoAction) {
            this.Do(action, undoAction, null);
        }

        /// <inheritdoc />
        public void Do(Action action, Action undoAction, Action propertyChangedAction) {
            var undoCommand = new UndoCommand(action, undoAction, propertyChangedAction);
            lock (this._lock) {
                undoCommand.Do();
                this._undoStack.Push(undoCommand);
                this._redoStack.Clear();
                this.RaiseProperties();
            }
        }

        /// <inheritdoc />
        public void Redo() {
            lock (this._lock) {
                if (this.CanRedo) {
                    var command = this._redoStack.Pop();
                    command.Do();
                    this._undoStack.Push(command);
                    this.RaiseProperties();
                }
            }
        }

        /// <inheritdoc />
        public void Undo() {
            lock (this._lock) {
                if (this.CanUndo) {
                    var command = this._undoStack.Pop();
                    command.Undo();
                    this._redoStack.Push(command);
                    this.RaiseProperties();
                }
            }
        }

        private void RaiseProperties() {
            this.RaisePropertyChanged(nameof(this.CanRedo));
            this.RaisePropertyChanged(nameof(this.CanUndo));
        }
    }
}