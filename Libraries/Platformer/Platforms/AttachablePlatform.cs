namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A base interface for all platforms.
/// </summary>
public interface IAttachablePlatform : ISimplePhysicsBody {
    /// <summary>
    /// Attaches the actor to this platform.
    /// </summary>
    /// <param name="actor">The actor to attach.</param>
    void Attach(IPlatformerActor actor);

    /// <summary>
    /// Detaches the actor from this platform.
    /// </summary>
    /// <param name="actor">The actor to detach.</param>
    void Detach(IPlatformerActor actor);
}

public class AttachablePlatform : Platform, IAttachablePlatform {
    private readonly HashSet<IPlatformerActor> _attached = new();

    /// <summary>
    /// Gets the attached actors.
    /// </summary>
    protected IReadOnlyCollection<IPlatformerActor> Attached => this._attached;

    /// <inheritdoc />
    public void Attach(IPlatformerActor actor) {
        this._attached.Add(actor);
    }

    /// <inheritdoc />
    public void Detach(IPlatformerActor actor) {
        this._attached.Remove(actor);
    }
}