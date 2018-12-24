namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public class Spinner : BaseComponent, IUpdateableComponent {

        public void Update(GameTime gameTime) {
            this.LocalRotation.Angle += gameTime.ElapsedGameTime.Milliseconds * 0.001f;
        }

        protected override void Initialize() {
            return;
        }
    }
}