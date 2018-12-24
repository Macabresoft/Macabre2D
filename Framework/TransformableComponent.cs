using Microsoft.Xna.Framework;

namespace Macabre2D.Framework {

    public class TransformableComponent : BaseComponent {

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        public override Matrix TransformMatrix {
            get {
                return this._transformMatrix.Value;
            }
        }

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <value>The world transform.</value>
        public override Transform WorldTransform {
            get {
                if (!this._isTransformUpToDate) {
                    this._transform.UpdateTransform(this.TransformMatrix);
                    this._isTransformUpToDate = true;
                }

                return this._transform;
            }
        }

        /// <summary>
        /// Sets the world transform.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        public override void SetWorldTransform(Vector2 position, float angle) {
            var currentTransform = this.WorldTransform;
            var matrix =
                Matrix.CreateScale(currentTransform.Scale.X, currentTransform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(angle) *
                Matrix.CreateTranslation(position.X, position.Y, 0f);

            if (this.Parent != null) {
                matrix = matrix * Matrix.Invert(this.Parent.TransformMatrix);
            }

            var localTransform = matrix.ToTransform();

            this._localPosition = localTransform.Position;
            this._localRotation = localTransform.Rotation;
            this._localScale = localTransform.Scale;

            this.HandleMatrixOrTransformChanged();
        }

        internal override void HandleMatrixOrTransformChanged() {
            this._transformMatrix = this._transformMatrix.Reset(this.GetMatrix);
            this._isTransformUpToDate = false;
            base.HandleMatrixOrTransformChanged();
        }

        protected override void Initialize() {
            return;
        }

        private Matrix GetMatrix() {
            var transformMatrix =
                Matrix.CreateScale(this.LocalScale.X, this.LocalScale.Y, 1f) *
                Matrix.CreateRotationZ(this.LocalRotation.Angle) *
                Matrix.CreateTranslation(this.LocalPosition.X, this.LocalPosition.Y, 0f);

            if (this.Parent != null) {
                transformMatrix = transformMatrix * this.Parent.TransformMatrix;
            }

            return transformMatrix;
        }
    }
}