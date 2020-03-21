namespace Macabre2D.Engine.Windows.Services {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using System;
    using System.Threading.Tasks;

    public sealed class BusyService : NotifyPropertyChanged, IBusyService {
        private int _busyCount = 0;
        private int _showBusyIndicatorCount = 0;

        public bool IsBusy {
            get {
                return (this.BusyCount + this.ShowBusyIndicatorCount) > 0;
            }
        }

        public bool ShowBusyIndicator {
            get {
                return this._showBusyIndicatorCount > 0;
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

        private int ShowBusyIndicatorCount {
            get {
                return this._showBusyIndicatorCount;
            }

            set {
                var originalIsBusy = this.IsBusy;
                var originalShowBusyIndicator = this.ShowBusyIndicator;
                this._showBusyIndicatorCount = value;

                if (originalIsBusy != this.IsBusy) {
                    this.RaisePropertyChanged(nameof(this.IsBusy));
                }

                if (originalShowBusyIndicator != this.ShowBusyIndicator) {
                    this.RaisePropertyChanged(nameof(this.ShowBusyIndicator));
                }
            }
        }

        public void PerformAction(Action action, bool showBusyIndicator) {
            try {
                this.Increment(showBusyIndicator);
                action.SafeInvoke();
            }
            finally {
                this.Decrement(showBusyIndicator);
            }
        }

        public async Task PerformTask(Action action, bool showBusyIndicator) {
            await this.PerformTask(Task.Run(action), showBusyIndicator);
        }

        public async Task<T> PerformTask<T>(Func<T> function, bool showBusyIndicator) {
            return await this.PerformTask(Task.Run(function), showBusyIndicator);
        }

        public async Task PerformTask(Task task, bool showBusyIndicator) {
            try {
                this.Increment(showBusyIndicator);
                await task;
            }
            finally {
                this.Decrement(showBusyIndicator);
            }
        }

        public async Task<T> PerformTask<T>(Task<T> task, bool showBusyIndicator) {
            try {
                this.Increment(showBusyIndicator);
                return await task;
            }
            finally {
                this.Decrement(showBusyIndicator);
            }
        }

        private void Decrement(bool showBusyIndicator) {
            if (showBusyIndicator) {
                this.ShowBusyIndicatorCount--;
            }
            else {
                this.BusyCount--;
            }
        }

        private void Increment(bool showBusyIndicator) {
            if (showBusyIndicator) {
                this.ShowBusyIndicatorCount++;
            }
            else {
                this.BusyCount++;
            }
        }
    }
}