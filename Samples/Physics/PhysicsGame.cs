namespace Macabresoft.MonoGame.Samples.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PhysicsGame : DefaultGame {

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

            var scene = new GameScene();
            scene.AddSystem<UpdateSystem>();
            scene.AddSystem<RenderSystem>();
            var physicsService = scene.AddSystem<PhysicsSystem>();
            physicsService.Gravity.Value = new Vector2(0f, -9f);
            physicsService.TimeStep = 1f / 60f;

            scene.AddChild().AddComponent<CameraComponent>();

            var circleEntity = scene.AddChild();
            var circleBody = circleEntity.AddComponent<DynamicPhysicsBody>();
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
                    var smallCircleBody = smallCircleEntity.AddComponent<DynamicPhysicsBody>();
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
            var rectangleBody = rectangleEntity.AddComponent<SimplePhysicsBody>();
            rectangleBody.Collider = PolygonCollider.CreateRectangle(10f, 1f);
            rectangleBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            rectangleEntity.LocalPosition -= new Vector2(0f, 4f);
            var rectangleDrawer = rectangleEntity.AddComponent<ColliderDrawerComponent>();
            rectangleDrawer.Color = DefinedColors.MacabresoftBone;
            rectangleDrawer.LineThickness = 1f;

            var angleEntity1 = scene.AddChild();
            var angleBody1 = angleEntity1.AddComponent<SimplePhysicsBody>();
            angleBody1.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(-5f, -3.5f));
            angleBody1.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer1 = angleEntity1.AddComponent<ColliderDrawerComponent>();
            angleDrawer1.Color = DefinedColors.MacabresoftBone;
            angleDrawer1.LineThickness = 1f;

            var angleEntity2 = scene.AddChild();
            var angleBody2 = angleEntity2.AddComponent<SimplePhysicsBody>();
            angleBody2.Collider = new LineCollider(new Vector2(8f, 4f), new Vector2(5f, -3.5f));
            angleBody2.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var angleDrawer2 = angleEntity2.AddComponent<ColliderDrawerComponent>();
            angleDrawer2.Color = DefinedColors.MacabresoftBone;
            angleDrawer2.LineThickness = 1f;

            var lineEntity = scene.AddChild();
            var lineBody = lineEntity.AddComponent<SimplePhysicsBody>();
            lineBody.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(8f, 4f));
            lineBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
            var lineDrawer = lineEntity.AddComponent<ColliderDrawerComponent>();
            lineDrawer.Color = DefinedColors.MacabresoftBone;
            lineDrawer.LineThickness = 1f;

            var triggerEntity = scene.AddChild();
            var triggerBody = triggerEntity.AddComponent<SimplePhysicsBody>();
            triggerBody.Collider = PolygonCollider.CreateRectangle(2f, 2f);
            triggerEntity.LocalPosition += new Vector2(2f, 2.5f);
            triggerBody.IsTrigger = true;
            triggerEntity.AddComponent<TriggerListener>();
            var triggerDrawer = triggerEntity.AddComponent<ColliderDrawerComponent>();
            triggerDrawer.Color = DefinedColors.MacabresoftRed;
            triggerDrawer.LineThickness = 3f;

            scene.Initialize(this);

            Serializer.Instance.Serialize(scene, @"Physics Game - Scene.json");
            scene = Serializer.Instance.Deserialize<GameScene>(@"Physics Game - Scene.json");
            this.LoadScene(scene);
        }
    }
}