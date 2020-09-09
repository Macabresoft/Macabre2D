namespace Macabresoft.MonoGame.Samples.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;

    public sealed class TriggerListener : Core.GameComponent {
        private IPhysicsBody _body;
        private ColliderDrawerComponent _drawer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.TryGetComponent<IPhysicsBody>(out var body) && body != null) {
                this._body = body;
            }
            else {
                this._body = PhysicsBody.Empty;
            }

            this._body.CollisionOccured += this._body_CollisionOccured;

            this.Entity.Scene.Invoke(() => {
                this._drawer = this.Entity.AddComponent<ColliderDrawerComponent>();
                this._drawer.Color = Color.MonoGameOrange;
                this._drawer.LineThickness = 3f;
            });
        }

        private void _body_CollisionOccured(object sender, CollisionEventArgs e) {
            if (this._drawer != null) {
                this._drawer.Color = Color.CornflowerBlue;
                this._drawer.LineThickness = 5f;
            }
        }
    }
}