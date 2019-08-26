namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;

    public interface IUndoService : INotifyPropertyChanged {
        bool CanRedo { get; }

        bool CanUndo { get; }

        void Clear();

        void Do(UndoCommand undoCommand);

        void Redo();

        void Undo();
    }
}