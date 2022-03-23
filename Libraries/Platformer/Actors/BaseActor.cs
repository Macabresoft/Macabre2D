namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A base interface for actors.
/// </summary>
public interface IBaseActor {
    /// <summary>
    /// Gets the current state of this actor.
    /// </summary>
    ActorState CurrentState { get; }

    /// <summary>
    /// Gets the previous state of this actor.
    /// </summary>
    ActorState PreviousState { get; }
    
    /// <summary>
    /// Gets the update order.
    /// </summary>
    public int UpdateOrder { get; }
}

/// <summary>
/// A base class for actors.
/// </summary>
public abstract class BaseActor : UpdateableEntity, IBaseActor {
    /// <summary>
    /// Gets an empty <see cref="IBaseActor"/>.
    /// </summary>
    public new static readonly IBaseActor Empty = new EmptyBaseActor();
    
    private ActorState _currentState;
    private ActorState _previousState;

    /// <inheritdoc />
    public ActorState CurrentState {
        get => this._currentState;
        protected set => this.Set(ref this._currentState, value);
    }

    /// <inheritdoc />
    public ActorState PreviousState {
        get => this._previousState;
        protected set => this.Set(ref this._previousState, value);
    }

    private sealed class EmptyBaseActor : IBaseActor {
        public ActorState CurrentState => ActorState.Default;
        public ActorState PreviousState => ActorState.Default;
        
        public int UpdateOrder => 0;
    }
}