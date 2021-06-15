namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws a collider.
    /// </summary>
    [Display(Name = "Collider Drawer (Diagnostics)")]
    public sealed class ColliderDrawer : BaseDrawer, IUpdateableEntity {
        private IPhysicsBody? _body;

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._body?.BoundingArea ?? BoundingArea.Empty;

        /// <inheritdoc />
        public int UpdateOrder => 0;

        public override void Initialize(IScene scene, IEntity parent) {
            base.Initialize(scene, parent);
            this.Reset();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this._body != null && this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var colliders = this._body.GetColliders();

                foreach (var collider in colliders) {
                    this.PrimitiveDrawer.DrawCollider(
                        collider,
                        spriteBatch,
                        this.Scene.Game.Project.Settings.PixelsPerUnit,
                        this.Color,
                        lineThickness,
                        Vector2.Zero);
                }
            }
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            this.Reset();
        }

        private void Reset() {
            this._body = this.Parent as IPhysicsBody;
        }
    }
}