namespace Macabre2D.UI.Library.ServiceInterfaces {

    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface IBusyService : INotifyPropertyChanged {
        bool IsBusy { get; }

        bool ShowBusyIndicator { get; }

        void PerformAction(Action action, bool showBusyIndicator);

        Task PerformTask(Action action, bool showBusyIndicator);

        Task PerformTask(Task task, bool showBusyIndicator);

        Task<T> PerformTask<T>(Func<T> function, bool showBusyIndicator);

        Task<T> PerformTask<T>(Task<T> task, bool showBusyIndicator);
    }
}