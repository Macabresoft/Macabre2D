namespace Macabresoft.Macabre2D.Samples.Physics {

    using Macabresoft.Macabre2D.Framework;

    public sealed class TriggerListener : Framework.GameComponent {
        private IPhysicsBodyComponent _body;
        private ColliderDrawerComponent _drawer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.TryGetComponent<IPhysicsBodyComponent>(out var body) && body != null) {
                this._body = body;
            }

            this._body.CollisionOccured += this.Body_CollisionOccured;
            this.Entity.TryGetComponent(out this._drawer);
        }

        private void Body_CollisionOccured(object sender, CollisionEventArgs e) {
            if (this._drawer != null) {
                this._drawer.Color = DefinedColors.ZvukostiGreen;
                this._drawer.LineThickness = 5f;
            }
        }
    }
}