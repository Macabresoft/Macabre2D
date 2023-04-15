namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// A reference to a loop using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TLoop">The type of loop.</typeparam>
public class LoopReference<TLoop> : PropertyChangedNotifier where TLoop : class, ILoop {
    private TLoop? _loop;
    private Guid _loopId;
    private IScene _scene = Scene.Empty;

    /// <summary>
    /// Gets the loop.
    /// </summary>
    public TLoop? Loop {
        get => this._loop;
        private set => this.Set(ref this._loop, value);
    }

    /// <summary>
    /// Gets or sets the loop identifier.
    /// </summary>
    public Guid LoopId {
        get => this._loopId;
        set {
            if (this.Set(ref this._loopId, value)) {
                this.SetLoop();
            }
        }
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="scene">The scene.</param>
    public void Initialize(IScene scene) {
        this._scene = scene;
        this.SetLoop();
    }

    private void SetLoop() {
        if (this._loopId == Guid.Empty || Scene.IsNullOrEmpty(this._scene)) {
            this.Loop = null;
        }
        else {
            this.Loop = this._scene.FindLoop<TLoop>(this._loopId);
        }
    }
}