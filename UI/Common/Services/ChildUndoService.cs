namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System.Linq;
    using Macabresoft.Macabre2D.UI.Common.Models;

    /// <summary>
    /// Interface for a child undo service. Generally an undo service that operates on a dialog and is committed into the main
    /// undo service later.
    /// </summary>
    public interface IChildUndoService : IUndoService {
        /// <summary>
        /// Gets a command that can be used to undo and redo all changes from this service.
        /// </summary>
        /// <returns>A command that can be used to undo and redo all changes from this service.</returns>
        UndoCommand GetChanges();
    }

    /// <summary>
    /// A child undo service. Generally an undo service that operates on a dialog and is committed into the main undo service
    /// later.
    /// </summary>
    public sealed class ChildUndoService : UndoService, IChildUndoService {
        /// <inheritdoc />
        public UndoCommand GetChanges() {
            UndoCommand undoCommand;
            if (this.CanUndo) {
                var undoStack = this.UndoStack.ToList();
                var redoStack = this.UndoStack.Reverse().ToList();

                undoCommand = new UndoCommand(
                    () => redoStack.ForEach(x => x.Do()),
                    () => undoStack.ForEach(x => x.Undo()));
            }
            else {
                undoCommand = null;
            }

            return undoCommand;
        }
    }
}