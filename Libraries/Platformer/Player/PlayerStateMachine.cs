namespace Macabresoft.Macabre2D.Libraries.Platformer.Player;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A state machine for the player. 
/// </summary>
public class PlayerStateMachine : UpdateableEntity {
    private PlayerState _currentState ;
    private LoopingSpriteAnimator? _spriteAnimator;

    /// <summary>
    /// Gets the current state of this machine.
    /// </summary>
    public PlayerState CurrentState {
        get => this._currentState;
        private set => this.Set(ref this._currentState, value, true);
    }
    
    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this._spriteAnimator == null) {
            return;
        }
        
        // TODO: check user input and collision status
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._spriteAnimator = this.GetOrAddChild<LoopingSpriteAnimator>();
    }
}