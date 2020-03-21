namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    using Macabre2D.Engine.Windows.Models;
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