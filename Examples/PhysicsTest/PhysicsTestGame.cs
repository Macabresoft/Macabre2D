namespace Macabre2D.Examples.PhysicsTest {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PhysicsTestGame : MacabreGame {

        protected override void Initialize() {
            this._graphics.PreferredBackBufferHeight = 1080;
            this._graphics.PreferredBackBufferWidth = 1920;
            this.IsFixedTimeStep = false;
            base.Initialize();
        }

        protected override void LoadContent() {
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

            var scene = new Scene();

            var camera = new Camera();
            scene.AddComponent(camera);

            var physicsModule = scene.AddModule<PhysicsModule>(1f / 60f);
            physicsModule.Gravity = new Gravity(new Vector2(0f, -9f));

            var circleBody = new DynamicBody();
            circleBody.LocalPosition -= new Vector2(0f, 3f);
            circleBody.IsKinematic = true;
            circleBody.Mass = 3f;
            var circleCollider = new CircleCollider(0.75f);
            circleBody.Collider = circleCollider;
            var circleDrawer = new ColliderDrawer();
            circleBody.AddChild(circleDrawer);
            circleDrawer.Color = Color.Green;
            circleDrawer.LineThickness = 2f;
            circleBody.AddChild(new VelocityChanger());
            scene.AddComponent(circleBody);

            for (var y = 0; y < 1; y++) {
                for (var x = 0; x < 1; x++) {
                    var smallCircleBody = new DynamicBody { Name = $"small circle {x}" };
                    smallCircleBody.LocalPosition -= new Vector2(-3 + x, -1f + y);
                    smallCircleBody.IsKinematic = true;
                    smallCircleBody.Mass = 1f;
                    var smallCircleCollider = new CircleCollider(0.3f);
                    smallCircleBody.Collider = smallCircleCollider;
                    smallCircleBody.PhysicsMaterial = new PhysicsMaterial(1f, 0f);
                    var smallCircleDrawer = new ColliderDrawer();
                    smallCircleBody.AddChild(smallCircleDrawer);
                    smallCircleDrawer.Color = Color.OrangeRed;
                    smallCircleDrawer.LineThickness = 1f;
                    scene.AddComponent(smallCircleBody);
                }
            }

            var rectangleBody = new Body();
            rectangleBody.Collider = PolygonCollider.CreateRectangle(10f, 1f);
            rectangleBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            rectangleBody.LocalPosition -= new Vector2(0f, 4f);
            var rectangleDrawer = new ColliderDrawer();
            rectangleBody.AddChild(rectangleDrawer);
            rectangleDrawer.Color = Color.White;
            rectangleDrawer.LineThickness = 1f;
            scene.AddComponent(rectangleBody);

            var angleBody1 = new Body();
            angleBody1.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(-5f, -3.5f));
            angleBody1.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer1 = new ColliderDrawer();
            angleBody1.AddChild(angleDrawer1);
            angleDrawer1.Color = Color.White;
            angleDrawer1.LineThickness = 1f;
            scene.AddComponent(angleBody1);

            var angleBody2 = new Body();
            angleBody2.Collider = new LineCollider(new Vector2(8f, 4f), new Vector2(5f, -3.5f));
            angleBody2.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer2 = new ColliderDrawer();
            angleBody2.AddChild(angleDrawer2);
            angleDrawer2.Color = Color.White;
            angleDrawer2.LineThickness = 1f;
            scene.AddComponent(angleBody2);

            var lineBody = new Body();
            lineBody.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(8f, 4f));
            lineBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var lineDrawer = new ColliderDrawer();
            lineBody.AddChild(lineDrawer);
            lineDrawer.Color = Color.White;
            lineDrawer.LineThickness = 1f;
            scene.AddComponent(lineBody);

            scene.SaveToFile(@"TestGame - CurrentLevel.json", new Serializer());
            this.CurrentScene = new Serializer().Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this._isLoaded = true;
        }
    }
}