namespace Macabre2D.Samples.Physics {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PhysicsGame : MacabreGame {

        protected override void ApplyGraphicsSettings() {
            base.ApplyGraphicsSettings();
        }

        protected override void Initialize() {
            this._graphics.PreferredBackBufferHeight = 1080;
            this._graphics.PreferredBackBufferWidth = 1920;
            this._graphics.ApplyChanges();
            this.IsFixedTimeStep = false;
            base.Initialize();
        }

        protected override void LoadContent() {
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

            var scene = new Scene();

            var camera = new Camera();
            scene.AddChild(camera);

            var physicsModule = scene.CreateModule<PhysicsModule>(1f / 60f);
            physicsModule.Gravity.Value = new Vector2(0f, -9f);

            var circleBody = new DynamicBodyComponent();
            circleBody.LocalPosition -= new Vector2(0f, 3f);
            circleBody.IsKinematic = true;
            circleBody.Mass = 3f;
            var circleCollider = new CircleCollider(0.75f);
            circleBody.Collider = circleCollider;
            var circleDrawer = new ColliderDrawerComponent();
            circleBody.AddChild(circleDrawer);
            circleDrawer.Color = Color.Green;
            circleDrawer.LineThickness = 2f;
            circleBody.AddChild(new VelocityChanger());
            scene.AddChild(circleBody);

            for (var y = 0; y < 1; y++) {
                for (var x = 0; x < 1; x++) {
                    var smallCircleBody = new DynamicBodyComponent { Name = $"small circle {x}" };
                    smallCircleBody.LocalPosition -= new Vector2(-3 + x, -1f + y);
                    smallCircleBody.IsKinematic = true;
                    smallCircleBody.Mass = 1f;
                    var smallCircleCollider = new CircleCollider(0.3f);
                    smallCircleBody.Collider = smallCircleCollider;
                    smallCircleBody.PhysicsMaterial = new PhysicsMaterial(1f, 0f);
                    var smallCircleDrawer = new ColliderDrawerComponent();
                    smallCircleBody.AddChild(smallCircleDrawer);
                    smallCircleDrawer.Color = Color.OrangeRed;
                    smallCircleDrawer.LineThickness = 1f;
                    scene.AddChild(smallCircleBody);
                }
            }

            var rectangleBody = new SimpleBodyComponent();
            rectangleBody.Collider = PolygonCollider.CreateRectangle(10f, 1f);
            rectangleBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            rectangleBody.LocalPosition -= new Vector2(0f, 4f);
            var rectangleDrawer = new ColliderDrawerComponent();
            rectangleBody.AddChild(rectangleDrawer);
            rectangleDrawer.Color = Color.White;
            rectangleDrawer.LineThickness = 1f;
            scene.AddChild(rectangleBody);

            var angleBody1 = new SimpleBodyComponent();
            angleBody1.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(-5f, -3.5f));
            angleBody1.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer1 = new ColliderDrawerComponent();
            angleBody1.AddChild(angleDrawer1);
            angleDrawer1.Color = Color.White;
            angleDrawer1.LineThickness = 1f;
            scene.AddChild(angleBody1);

            var angleBody2 = new SimpleBodyComponent();
            angleBody2.Collider = new LineCollider(new Vector2(8f, 4f), new Vector2(5f, -3.5f));
            angleBody2.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer2 = new ColliderDrawerComponent();
            angleBody2.AddChild(angleDrawer2);
            angleDrawer2.Color = Color.White;
            angleDrawer2.LineThickness = 1f;
            scene.AddChild(angleBody2);

            var lineBody = new SimpleBodyComponent();
            lineBody.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(8f, 4f));
            lineBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var lineDrawer = new ColliderDrawerComponent();
            lineBody.AddChild(lineDrawer);
            lineDrawer.Color = Color.White;
            lineDrawer.LineThickness = 1f;
            scene.AddChild(lineBody);

            var triggerBody = new SimpleBodyComponent();
            triggerBody.Collider = PolygonCollider.CreateRectangle(2f, 2f);
            triggerBody.LocalPosition += new Vector2(2f, 2.5f);
            triggerBody.IsTrigger = true;
            var triggerListener = new TriggerListener();
            triggerBody.AddChild(triggerListener);
            scene.AddChild(triggerBody);

            scene.SaveToFile(@"TestGame - CurrentLevel.json");
            this.CurrentScene = Serializer.Instance.Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this._isLoaded = true;
        }
    }
}