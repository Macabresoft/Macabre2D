namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A shader that wraps around <see cref="Effect"/>.
    /// </summary>
    public sealed class Shader : BaseIdentifiable, IAsset, IDisposable {

        /// <inheritdoc/>
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
        public Effect Effect { get; set; }

        /// <inheritdoc/>
        public void Dispose() {
            this.Effect?.Dispose();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void Load() {
            if (this.AssetId != Guid.Empty) {
                this.Effect = AssetManager.Instance.Load<Effect>(this.AssetId);
            }
        }
    }
}