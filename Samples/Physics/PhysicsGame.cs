namespace Macabresoft.Macabre2D.Samples.Physics {
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [ExcludeFromCodeCoverage]
    public class PhysicsGame : BaseGame {
        protected override void Initialize() {
            this._graphics.PreferredBackBufferHeight = 1080;
            this._graphics.PreferredBackBufferWidth = 1920;
            this._graphics.ApplyChanges();
            this.IsFixedTimeStep = false;
            base.Initialize();
        }

        protected override void LoadContent() {
            this.Project.Assets.Initialize(this.Content, Serializer.Instance);
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

            var scene = new GameScene();
            scene.AddSystem<UpdateSystem>();
            scene.AddSystem<RenderSystem>();
            var physicsService = scene.AddSystem<PhysicsSystem>();
            physicsService.Gravity.Value = new Vector2(0f, -9f);
            physicsService.TimeStep = 1f / 60f;

            var leagueMono = new Font();
            this.Project.Assets.RegisterMetadata(new ContentMetadata(leagueMono, new[] { "League Mono" }, ".spritefont"));

            var cameraEntity = scene.AddChild();
            cameraEntity.AddComponent<CameraComponent>();
            var frameRateDisplayEntity = cameraEntity.AddChild();
            var frameRateDisplay = frameRateDisplayEntity.AddComponent<FrameRateDisplayComponent>();
            frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
            frameRateDisplayEntity.LocalScale = new Vector2(0.1f);
            frameRateDisplay.FontReference.Initialize(leagueMono);

            var circleEntity = scene.AddChild();
            var circleBody = circleEntity.AddComponent<DynamicPhysicsBodyComponent>();
            circleEntity.LocalPosition -= new Vector2(0f, 3f);
            circleBody.IsKinematic = true;
            circleBody.Mass = 3f;
            circleBody.Collider = new CircleCollider(0.75f);
            var circleDrawer = circleEntity.AddComponent<ColliderDrawerComponent>();
            circleDrawer.Color = DefinedColors.MacabresoftYellow;
            circleDrawer.LineThickness = 2f;
            circleEntity.AddComponent<VelocityChanger>();

            for (var y = 0; y < 2; y++) {
                for (var x = 0; x < 4; x++) {
                    var smallCircleEntity = scene.AddChild();
                    smallCircleEntity.Name = $"small circle ({x}, {y})";
                    var smallCircleBody = smallCircleEntity.AddComponent<DynamicPhysicsBodyComponent>();
                    smallCircleEntity.LocalPosition -= new Vector2(-3 + x, -1f + y);
                    smallCircleBody.IsKinematic = true;
                    smallCircleBody.Mass = 1f;
                    smallCircleBody.Collider = new CircleCollider(0.3f);
                    smallCircleBody.PhysicsMaterial = new PhysicsMaterial(1f, 0f);

                    var smallCircleDrawer = smallCircleEntity.AddComponent<ColliderDrawerComponent>();
                    smallCircleDrawer.Color = DefinedColors.MacabresoftPurple;
                    smallCircleDrawer.LineThickness = 1f;
                }
            }

            var rectangleEntity = scene.AddChild();
            var rectangleBody = rectangleEntity.AddComponent<SimplePhysicsBodyComponent>();
            rectangleBody.Collider = PolygonCollider.CreateRectangle(10f, 1f);
            rectangleBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            rectangleEntity.LocalPosition -= new Vector2(0f, 4f);
            var rectangleDrawer = rectangleEntity.AddComponent<ColliderDrawerComponent>();
            rectangleDrawer.Color = DefinedColors.MacabresoftBone;
            rectangleDrawer.LineThickness = 1f;

            var angleEntity1 = scene.AddChild();
            var angleBody1 = angleEntity1.AddComponent<SimplePhysicsBodyComponent>();
            angleBody1.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(-5f, -3.5f));
            angleBody1.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer1 = angleEntity1.AddComponent<ColliderDrawerComponent>();
            angleDrawer1.Color = DefinedColors.MacabresoftBone;
            angleDrawer1.LineThickness = 1f;

            var angleEntity2 = scene.AddChild();
            var angleBody2 = angleEntity2.AddComponent<SimplePhysicsBodyComponent>();
            angleBody2.Collider = new LineCollider(new Vector2(8f, 4f), new Vector2(5f, -3.5f));
            angleBody2.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer2 = angleEntity2.AddComponent<ColliderDrawerComponent>();
            angleDrawer2.Color = DefinedColors.MacabresoftBone;
            angleDrawer2.LineThickness = 1f;

            var lineEntity = scene.AddChild();
            var lineBody = lineEntity.AddComponent<SimplePhysicsBodyComponent>();
            lineBody.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(8f, 4f));
            lineBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var lineDrawer = lineEntity.AddComponent<ColliderDrawerComponent>();
            lineDrawer.Color = DefinedColors.MacabresoftBone;
            lineDrawer.LineThickness = 1f;

            var triggerEntity = scene.AddChild();
            var triggerBody = triggerEntity.AddComponent<SimplePhysicsBodyComponent>();
            triggerBody.Collider = PolygonCollider.CreateRectangle(2f, 2f);
            triggerEntity.LocalPosition += new Vector2(2f, 2.5f);
            triggerBody.IsTrigger = true;
            triggerEntity.AddComponent<TriggerListener>();
            var triggerDrawer = triggerEntity.AddComponent<ColliderDrawerComponent>();
            triggerDrawer.Color = DefinedColors.MacabresoftRed;
            triggerDrawer.LineThickness = 3f;

            scene.Initialize(this);

            var fileName = Path.GetTempFileName();
            Serializer.Instance.Serialize(scene, fileName);
            scene = Serializer.Instance.Deserialize<GameScene>(fileName);
            File.Delete(fileName);

            this.LoadScene(scene);
        }
    }
}