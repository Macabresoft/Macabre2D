namespace Macabresoft.MonoGame.Samples.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;

    public sealed class TriggerListener : BaseComponent {
        private IPhysicsBody _body;
        private ColliderDrawerComponent _drawer;

        protected override void Initialize() {
            this._body = this.Parent as IPhysicsBody;
            this._body.CollisionOccured += this._body_CollisionOccured;

            this._drawer = this.AddChild<ColliderDrawerComponent>();
            this._drawer.Body = this._body;
            this._drawer.Color = Color.MonoGameOrange;
            this._drawer.LineThickness = 3f;
        }

        private void _body_CollisionOccured(object sender, CollisionEventArgs e) {
            this._drawer.Color = Color.CornflowerBlue;
            this._drawer.LineThickness = 5f;
        }
    }
}