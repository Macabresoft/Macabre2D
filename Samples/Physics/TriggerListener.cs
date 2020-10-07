namespace Macabresoft.MonoGame.Samples.Physics {

    using Macabresoft.MonoGame.Core2D;

    public sealed class TriggerListener : Core2D.GameComponent {
        private IPhysicsBody _body;
        private ColliderDrawerComponent _drawer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.TryGetComponent<IPhysicsBody>(out var body) && body != null) {
                this._body = body;
            }

            this._body.CollisionOccured += this._body_CollisionOccured;
            this.Entity.TryGetComponent(out this._drawer);
        }

        private void _body_CollisionOccured(object sender, CollisionEventArgs e) {
            if (this._drawer != null) {
                this._drawer.Color = DefinedColors.ZvukostiGreen;
                this._drawer.LineThickness = 5f;
            }
        }
    }
}