namespace Macabresoft.Macabre2D.Samples.Physics;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

[ExcludeFromCodeCoverage]
public class PhysicsGame : BaseGame {
    private FontAsset _font;

    protected override void Initialize() {
        this.GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
        this.GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
        this.GraphicsDeviceManager.ApplyChanges();
        this.IsFixedTimeStep = false;
        base.Initialize();
    }

    protected override void LoadContent() {
        this.TryCreateSpriteBatch();

        var scene = new Scene();
        scene.AddLoop<UpdateLoop>();
        scene.AddLoop<RenderLoop>();
        var physicsService = scene.AddLoop<PhysicsLoop>();
        physicsService.Gravity.Value = new Vector2(0f, -9f);
        physicsService.TimeStep = 1f / 60f;

        this._font = new FontAsset();

        var cameraEntity = scene.AddChild<Camera>();
        var frameRateDisplay = cameraEntity.AddChild<FrameRateDisplayEntity>();
        frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
        frameRateDisplay.LocalScale = new Vector2(0.1f);
        frameRateDisplay.FontReference.LoadAsset(this._font);

        var circleEntity = scene.AddChild<DynamicPhysicsBody>();
        circleEntity.LocalPosition -= new Vector2(0f, 3f);
        circleEntity.IsKinematic = true;
        circleEntity.Mass = 3f;
        circleEntity.Collider = new CircleCollider(0.75f);
        var circleDrawer = circleEntity.AddChild<ColliderDrawer>();
        circleDrawer.Color = DefinedColors.MacabresoftYellow;
        circleDrawer.LineThickness = 2f;
        circleEntity.AddChild<VelocityChanger>();

        for (var y = 0; y < 2; y++) {
            for (var x = 0; x < 4; x++) {
                var smallCircleBody = scene.AddChild<DynamicPhysicsBody>();
                smallCircleBody.Name = $"small circle ({x}, {y})";
                smallCircleBody.LocalPosition -= new Vector2(-3 + x, -1f + y);
                smallCircleBody.IsKinematic = true;
                smallCircleBody.Mass = 1f;
                smallCircleBody.Collider = new CircleCollider(0.3f);
                smallCircleBody.PhysicsMaterial = new PhysicsMaterial(1f, 0f);

                var smallCircleDrawer = smallCircleBody.AddChild<ColliderDrawer>();
                smallCircleDrawer.Color = DefinedColors.MacabresoftPurple;
                smallCircleDrawer.LineThickness = 1f;
            }
        }

        var rectangleBody = scene.AddChild<SimplePhysicsBody>();
        rectangleBody.Collider = new RectangleCollider(new Vector2(-5f, -0.5f), new Vector2(5f, 0.5f));
        rectangleBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
        rectangleBody.LocalPosition -= new Vector2(0f, 4f);
        var rectangleDrawer = rectangleBody.AddChild<ColliderDrawer>();
        rectangleDrawer.Color = DefinedColors.MacabresoftBone;
        rectangleDrawer.LineThickness = 1f;

        var angleBody1 = scene.AddChild<SimplePhysicsBody>();
        angleBody1.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(-5f, -3.5f));
        angleBody1.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
        var angleDrawer1 = angleBody1.AddChild<ColliderDrawer>();
        angleDrawer1.Color = DefinedColors.MacabresoftBone;
        angleDrawer1.LineThickness = 1f;

        var angleBody2 = scene.AddChild<SimplePhysicsBody>();
        angleBody2.Collider = new LineCollider(new Vector2(8f, 4f), new Vector2(5f, -3.5f));
        angleBody2.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
        var angleDrawer2 = angleBody2.AddChild<ColliderDrawer>();
        angleDrawer2.Color = DefinedColors.MacabresoftBone;
        angleDrawer2.LineThickness = 1f;

        var lineBody = scene.AddChild<SimplePhysicsBody>();
        lineBody.Collider = new LineCollider(new Vector2(-8f, 4f), new Vector2(8f, 4f));
        lineBody.PhysicsMaterial = new PhysicsMaterial(0.5f, 1f);
        var lineDrawer = lineBody.AddChild<ColliderDrawer>();
        lineDrawer.Color = DefinedColors.MacabresoftBone;
        lineDrawer.LineThickness = 1f;

        var triggerBody = scene.AddChild<SimplePhysicsBody>();
        triggerBody.Collider = new RectangleCollider(new Vector2(-1f, -1f), new Vector2(1f, 1f));
        triggerBody.LocalPosition += new Vector2(2f, 2.5f);
        triggerBody.IsTrigger = true;
        triggerBody.AddChild<TriggerListener>();
        var triggerDrawer = triggerBody.AddChild<ColliderDrawer>();
        triggerDrawer.Color = DefinedColors.MacabresoftRed;
        triggerDrawer.LineThickness = 3f;

        scene.Initialize(this, this.CreateAssetManager());

        var fileName = Path.GetTempFileName();
        Serializer.Instance.Serialize(scene, fileName);
        scene = Serializer.Instance.Deserialize<Scene>(fileName);
        File.Delete(fileName);

        this.LoadScene(scene);
    }

    protected override void RegisterNewSceneMetadata(IAssetManager assetManager) {
        base.RegisterNewSceneMetadata(assetManager);
        assetManager.RegisterMetadata(new ContentMetadata(this._font, new[] { "League Mono" }, ".spritefont"));
    }
}