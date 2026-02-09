namespace Macabre2D.UI.Common;

using System;
using Macabresoft.Core;

/// <summary>
/// Interface for a service that indicates whether the program is currently busy.
/// </summary>
public interface IBusyService {
    /// <summary>
    /// Gets a value indicating whether or not the program is currently busy.
    /// </summary>
    public bool IsBusy { get; }

    /// <summary>
    /// Tries to claim the busy status of the program with a <see cref="IDisposable" /> that will reset <see cref="IsBusy" /> when disposed.
    /// </summary>
    /// <param name="disposable">The disposable.</param>
    /// <returns>A value indicating whether or not the busy status could be claimed.</returns>
    bool TryClaimBusy(out IDisposable disposable);
}

/// <summary>
/// A service that indicates whether the program is currently busy.
/// </summary>
public class BusyService : PropertyChangedNotifier, IBusyService {
    private bool _isBusy;

    /// <inheritdoc />
    public bool IsBusy {
        get => this._isBusy;
        private set => this.Set(ref this._isBusy, value);
    }

    /// <inheritdoc />
    public bool TryClaimBusy(out IDisposable disposable) {
        disposable = !this.IsBusy ? new BusyDisposable(this) : null;
        return disposable != null;
    }

    private sealed class BusyDisposable : IDisposable {
        private readonly BusyService _busyService;

        public BusyDisposable(BusyService busyService) {
            this._busyService = busyService;
            this._busyService.IsBusy = true;
        }

        public void Dispose() {
            this._busyService.IsBusy = false;
        }
    }
}