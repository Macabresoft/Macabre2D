namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A <see cref="BaseBody"/> which reacts to a <see cref="ITileable"/> parent and creates
    /// colliders based on the available grid.
    /// </summary>
    public sealed class TileableBody : BaseBody {
        private ITileable _tileable;

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public override bool HasCollider {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<Collider> GetColliders() {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            if (this.Parent != null) {
                this._tileable = this.Parent as ITileable;
            }

            this.ParentChanged += this.TileableBodyComponent_ParentChanged;
            this.ResetColliders();
        }

        private void ResetColliders() {
            throw new NotImplementedException();
        }

        private void TileableBodyComponent_ParentChanged(object sender, BaseComponent e) {
            this._tileable = e as ITileable;
            this.ResetColliders();
        }
    }
}