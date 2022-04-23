namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A base class for a <see cref="IBlock" /> which can be broken.
/// </summary>
public abstract class BreakableBlock : Block {
    private bool _isBroken;

    /// <inheritdoc />
    public override bool HasCollider => !this._isBroken && base.HasCollider;

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        return this._isBroken ? Enumerable.Empty<Collider>() : base.GetColliders();
    }

    /// <inheritdoc />
    public override void Hit() {
        if (!this._isBroken) {
            this._isBroken = true;
            this.OnBroken();
        }
    }

    /// <summary>
    /// Called when this block is broken.
    /// </summary>
    protected abstract void OnBroken();
}