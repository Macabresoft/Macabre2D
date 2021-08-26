namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Linq;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Common.Models;

    /// <summary>
    /// Interface for a child undo service. Generally an undo service that operates on a dialog and is committed into the main
    /// undo service later.
    /// </summary>
    public interface IChildUndoService : IUndoService {
        /// <summary>
        /// Occurs when a commit was requested.
        /// </summary>
        event EventHandler<UndoCommand> CommitRequested;

        /// <summary>
        /// Commits the changes of this child.
        /// </summary>
        void CommitChanges();
    }

    /// <summary>
    /// A child undo service. Generally an undo service that operates on a dialog and is committed into the main undo service
    /// later.
    /// </summary>
    public class ChildUndoService : UndoService, IChildUndoService {
        /// <inheritdoc />
        public event EventHandler<UndoCommand> CommitRequested;

        /// <inheritdoc />
        public void CommitChanges() {
            var undoStack = this.UndoStack.ToList();
            var redoStack = this.UndoStack.Reverse().ToList();

            var undoCommand = new UndoCommand(
                () => redoStack.ForEach(x => x.Undo()),
                () => undoStack.ForEach(x => x.Do()));

            this.CommitRequested.SafeInvoke(this, undoCommand);
        }

        /// <inheritdoc />
        public override void Dispose() {
            base.Dispose();
            this.CommitRequested = null;
        }
    }
}