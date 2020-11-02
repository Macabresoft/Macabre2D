namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {

    using Macabresoft.Macabre2D.Framework;
    using System;

    public sealed class CameraControlComponent : GameUpdateableComponent {
        private CameraComponent _camera;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (!this.Entity.TryGetComponent(out this._camera)) {
                throw new ArgumentNullException(nameof(this._camera));
            }
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            throw new NotImplementedException();
        }
    }
}