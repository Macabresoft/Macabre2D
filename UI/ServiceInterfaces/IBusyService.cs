namespace Macabre2D.UI.ServiceInterfaces {

    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface IBusyService : INotifyPropertyChanged {
        bool IsBusy { get; }

        void PerformAction(Action action);

        Task PerformTask(Action action);

        Task PerformTask(Task task);

        Task<T> PerformTask<T>(Func<T> function);

        Task<T> PerformTask<T>(Task<T> task);
    }
}