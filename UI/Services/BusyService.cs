namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Threading.Tasks;

    public sealed class BusyService : NotifyPropertyChanged, IBusyService {
        private int _busyCount = 0;

        public bool IsBusy {
            get {
                return this.BusyCount != 0;
            }
        }

        private int BusyCount {
            get {
                return this._busyCount;
            }

            set {
                var originalIsBusy = this.IsBusy;
                this._busyCount = value;

                if (originalIsBusy != this.IsBusy) {
                    this.RaisePropertyChanged(nameof(this.IsBusy));
                }
            }
        }

        public void PerformAction(Action action) {
            try {
                this.BusyCount++;
                action.SafeInvoke();
            }
            finally {
                this.BusyCount--;
            }
        }

        public async Task PerformTask(Action action) {
            await this.PerformTask(Task.Run(action));
        }

        public async Task<T> PerformTask<T>(Func<T> function) {
            return await this.PerformTask(Task.Run(function));
        }

        public async Task PerformTask(Task task) {
            try {
                this.BusyCount++;
                await task;
            }
            finally {
                this.BusyCount--;
            }
        }

        public async Task<T> PerformTask<T>(Task<T> task) {
            try {
                this.BusyCount++;
                return await task;
            }
            finally {
                this.BusyCount--;
            }
        }
    }
}