namespace Macabresoft.Macabre2D.Samples.Physics {

    using Macabresoft.Macabre2D.Framework;

    public sealed class TriggerListener : Entity {
        private IPhysicsBody _body;
        private ColliderDrawer _drawer;

        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);

            if (this.TryGetParentEntity<IPhysicsBody>(out var body) && body != null) {
                this._body = body;
                this._body.TryGetChild<ColliderDrawer>(out this._drawer);
            }

            this._body.CollisionOccured += this.Body_CollisionOccured;
        }

        private void Body_CollisionOccured(object sender, CollisionEventArgs e) {
            if (this._drawer != null) {
                this._drawer.Color = DefinedColors.ZvukostiGreen;
                this._drawer.LineThickness = 5f;
                this._body.CollisionOccured -= this.Body_CollisionOccured;
            }
        }
    }
}