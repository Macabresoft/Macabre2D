namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A shader that wraps around <see cref="Effect" />.
    /// </summary>
    public sealed class Shader : BaseIdentifiable, IAsset, IDisposable {

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader" /> class.
        /// </summary>
        public Shader() : this(Guid.NewGuid()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader" /> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        public Shader(Guid assetId) {
            this.AssetId = assetId;
        }

        /// <inheritdoc />
        public Guid AssetId {
            get {
                return this.Id;
            }

            set {
                this.Id = value;
            }
        }

        /// <summary>
        /// Gets or sets the effect.
        /// </summary>
        /// <value>The effect.</value>
        public Effect? Effect { get; set; }

        /// <inheritdoc />
        public void Dispose() {
            this.Effect?.Dispose();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void Load() {
            if (this.AssetId != Guid.Empty) {
                try {
                    this.Effect = AssetManager.Instance.Load<Effect>(this.AssetId);
                }
                catch {
                    // TODO: surface an error message or use an insane shader here.
                    this.Effect = null;
                }
            }
        }
    }
}