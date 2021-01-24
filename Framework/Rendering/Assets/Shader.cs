namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A shader that wraps around <see cref="Effect" />.
    /// </summary>
    public sealed class Shader : BaseContentAsset<Effect>, IAsset, IDisposable {
        
        /// <inheritdoc />
        public void Dispose() {
            this.Content?.Dispose();
            this.DisposePropertyChanged();
        }
    }
}