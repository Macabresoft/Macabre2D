namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ReactiveUI;

/// <summary>
/// Interface for a service which handles undo/redo operations.
/// </summary>
public interface IUndoService : IDisposable, INotifyPropertyChanged {
    /// <summary>
    /// Gets a value indicating whether or not a redo operation is possible.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Gets a value indicating whether or not an undo operation is possible.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Gets the most recent change identifier. If no changes, will be <see cref="Guid.Empty" />.
    /// </summary>
    Guid LatestChangeId { get; }

    /// <summary>
    /// Clears the queue of undo and redo operations.
    /// </summary>
    void Clear();

    /// <summary>
    /// Commits external changes which have already been performed.
    /// </summary>
    /// <param name="command">The external changes as a command.</param>
    void CommitExternalChanges(UndoCommand command);

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
    private readonly object _lock = new();

    private readonly Stack<UndoCommand> _redoStack = new(50);
    private readonly Stack<UndoCommand> _undoStack = new(50);

    /// <inheritdoc />
    public bool CanRedo {
        get {
            lock (this._lock) {
                return this._redoStack.Any();
            }
        }
    }

    /// <inheritdoc />
    public bool CanUndo {
        get {
            lock (this._lock) {
                return this._undoStack.Any();
            }
        }
    }

    /// <inheritdoc />
    public Guid LatestChangeId => this._undoStack.Any() ? this._undoStack.Peek().ChangeId : Guid.Empty;

    /// <summary>
    /// Gets a stack of all undo operations associated with this service.
    /// </summary>
    protected IReadOnlyCollection<UndoCommand> UndoStack => this._undoStack;

    /// <inheritdoc />
    public void Clear() {
        lock (this._lock) {
            this._redoStack.Clear();
            this._undoStack.Clear();
            this.RaiseProperties();
        }
    }

    /// <inheritdoc />
    public void CommitExternalChanges(UndoCommand command) {
        if (command != null) {
            this.CommitCommand(command);
        }
    }

    /// <inheritdoc />
    public virtual void Dispose() {
        this._undoStack.Clear();
        this._redoStack.Clear();
    }

    /// <inheritdoc />
    public void Do(Action action, Action undoAction) {
        this.Do(action, undoAction, null);
    }

    /// <inheritdoc />
    public void Do(Action action, Action undoAction, Action propertyChangedAction) {
        var command = new UndoCommand(action, undoAction, propertyChangedAction);
        this.CommitCommand(command);
        command.Do();
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

    private void CommitCommand(UndoCommand command) {
        lock (this._lock) {
            this._undoStack.Push(command);
            this._redoStack.Clear();
            this.RaiseProperties();
        }
    }

    private void RaiseProperties() {
        this.RaisePropertyChanged(nameof(this.CanRedo));
        this.RaisePropertyChanged(nameof(this.CanUndo));
        this.RaisePropertyChanged(nameof(this.LatestChangeId));
    }
}