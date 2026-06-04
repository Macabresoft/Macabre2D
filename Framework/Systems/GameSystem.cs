namespace Macabre2D.Framework;

/// <summary>
/// Interface for a system which will persist at the game level, even when changing scenes.
/// </summary>
public interface IGameSystem : IBaseSystem {
    /// <summary>
    /// Initializes this system as a descendent of <paramref name="game" />.
    /// </summary>
    /// <param name="game">The game.</param>
    void Initialize(IGame game);
}

/// <summary>
/// A system which will persist at the game level, even when changing scenes.
/// </summary>
public abstract class GameSystem : BaseSystem, IGameSystem {
    /// <summary>
    /// Gets the game.
    /// </summary>
    protected IGame Game { get; private set; } = BaseGame.Empty;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.Game = BaseGame.Empty;
    }

    /// <inheritdoc />
    public virtual void Initialize(IGame game) {
        this.Game = game;
    }
}