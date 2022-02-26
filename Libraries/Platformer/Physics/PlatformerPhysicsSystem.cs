namespace Macabresoft.Macabre2D.Libraries.Platformer.Physics;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a physics engine for a platformer game.
/// </summary>
public interface IPlatformerPhysicsSystem : IPhysicsSystem {
    /// <summary>
    /// Gets the maximum downward velocity when falling.
    /// </summary>
    float TerminalVelocity { get; }

    /// <summary>
    /// Gets or sets the ceiling layer.
    /// </summary>
    public Layers CeilingLayer { get; set; }

    /// <summary>
    /// Gets or sets the ground layer.
    /// </summary>
    public Layers GroundLayer { get; set; }

    /// <summary>
    /// Gets or sets the wall layer.
    /// </summary>
    public Layers WallLayer { get; set; }
}

/// <summary>
/// A physics system for a platformer game.
/// </summary>
public class PlatformerPhysicsSystem : PhysicsSystem, IPlatformerPhysicsSystem {
    public static readonly IPlatformerPhysicsSystem Empty = new EmptyPlatformerPhysicsSystem();

    private Layers _ceilingLayer;
    private Layers _groundLayer;
    private float _terminalVelocity = 15f;
    private Layers _wallLayer;

    /// <inheritdoc />
    [DataMember]
    public Layers CeilingLayer {
        get => this._ceilingLayer;
        set => this.Set(ref this._ceilingLayer, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Layers GroundLayer {
        get => this._groundLayer;
        set => this.Set(ref this._groundLayer, value);
    }

    /// <inheritdoc />
    [DataMember]
    public float TerminalVelocity {
        get => this._terminalVelocity;
        protected set => this.Set(ref this._terminalVelocity, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Layers WallLayer {
        get => this._wallLayer;
        set => this.Set(ref this._wallLayer, value);
    }

    private sealed class EmptyPlatformerPhysicsSystem : IPlatformerPhysicsSystem {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public Gravity Gravity { get; } = new(Vector2.Zero);

        /// <inheritdoc />
        public float Groundedness => 0f;

        /// <inheritdoc />
        public SystemKind Kind => SystemKind.Update;

        /// <inheritdoc />
        public float MinimumPostBounceMagnitude => 0f;

        /// <inheritdoc />
        public float MinimumPostFrictionMagnitude => 0f;

        /// <inheritdoc />
        public float Stickiness => 0f;

        /// <inheritdoc />
        public float TerminalVelocity => 0f;

        /// <inheritdoc />
        public Layers CeilingLayer {
            get => Layers.None;
            set { }
        }

        /// <inheritdoc />
        public Layers GroundLayer {
            get => Layers.None;
            set { }
        }

        /// <inheritdoc />
        public bool IsEnabled {
            get => false;
            set { }
        }

        /// <inheritdoc />
        public string Name {
            get => string.Empty;
            set { }
        }

        /// <inheritdoc />
        public Layers WallLayer {
            get => Layers.None;
            set { }
        }

        /// <inheritdoc />
        public void Initialize(IScene scene) {
        }

        /// <inheritdoc />
        public IReadOnlyList<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers) {
            return Array.Empty<RaycastHit>();
        }

        /// <inheritdoc />
        public bool TryRaycast(Vector2 start, Vector2 direction, float distance, Layers layers, out RaycastHit hit) {
            hit = RaycastHit.Empty;
            return false;
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
        }
    }
}