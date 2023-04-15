namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// Base class for loop references.
/// </summary>
public abstract class LoopReference : PropertyChangedNotifier {
    private Guid _loopId;

    /// <summary>
    /// Gets or sets the loop identifier.
    /// </summary>
    public Guid LoopId {
        get => this._loopId;
        set {
            if (this._loopId != value) {
                this._loopId = value;
                this.ResetLoop();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets an untyped version of the loop.
    /// </summary>
    public ILoop? UntypedLoop { get; protected set; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = Framework.Scene.Empty;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="scene">The scene.</param>
    public void Initialize(IScene scene) {
        this.Scene = scene;
        this.ResetLoop();
    }

    /// <summary>
    /// Sets the loop.
    /// </summary>
    protected abstract void ResetLoop();
}

/// <summary>
/// A reference to a loop using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TLoop">The type of loop.</typeparam>
public class LoopReference<TLoop> : LoopReference where TLoop : class, ILoop {
    private TLoop? _loop;

    /// <summary>
    /// Gets the loop.
    /// </summary>
    public TLoop? Loop {
        get => this._loop;
        private set => this.Set(ref this._loop, value);
    }

    /// <inheritdoc />
    protected override void ResetLoop() {
        if (this.LoopId == Guid.Empty || Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Loop = null;
        }
        else {
            this.Loop = this.Scene.FindLoop<TLoop>(this.LoopId);
        }

        this.UntypedLoop = this.Loop;
    }
}