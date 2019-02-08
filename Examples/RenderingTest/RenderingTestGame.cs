namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Rendering;
    using Microsoft.Xna.Framework;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class RenderingTestGame : MacabreGame {

        protected override void Initialize() {
            base.Initialize();

            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            base.LoadContent();
            var scene = new Scene();

            var cameraScroller = new CameraScroller();
            scene.AddChild(cameraScroller);

            var camera = new Camera();
            cameraScroller.AddChild(camera);

            var movingDot = new MovingDot();
            camera.AddChild(movingDot);
            var spriteRenderer = new SpriteRenderer();
            spriteRenderer.Sprite = new Sprite("WhiteSquare", this.Content, Point.Zero, new Point(32, 32));
            spriteRenderer.OffsetType = OffsetType.Center;
            movingDot.AddChild(spriteRenderer);

            var spriteAnimation = new SpriteAnimation();
            var step = spriteAnimation.AddStep();
            step.Sprite = new Sprite("ColoredSquares", this.Content, Point.Zero, new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite("ColoredSquares", this.Content, new Point(0, 64), new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite("ColoredSquares", this.Content, new Point(64, 64), new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite("ColoredSquares", this.Content, new Point(64, 0), new Point(64, 64));
            step.Frames = 2;

            var spriteAnimator = new SpriteAnimator(spriteAnimation) {
                FrameRate = 4
            };

            var spriteRenderer2 = spriteAnimator.AddChild<SpriteRenderer>();
            spriteRenderer2.DrawOrder = -100;
            spriteRenderer2.OffsetType = OffsetType.Center;
            scene.AddChild(spriteAnimator);

            var spinner = new Spinner();
            spinner.LocalPosition -= new Vector2(2f, 0);
            scene.AddChild(spinner);
            var spriteRenderer3 = new SpriteRenderer();
            spinner.AddChild(spriteRenderer3);
            spriteRenderer3.DrawOrder = -200;
            spriteRenderer3.Sprite = new Sprite("WhiteSquare", this.Content, Point.Zero, new Point(32, 32));
            spriteRenderer3.OffsetType = OffsetType.Center;

            var middleSpinningDotBoundingArea = new BoundingAreaDrawer();
            spriteRenderer3.AddChild(middleSpinningDotBoundingArea);
            middleSpinningDotBoundingArea.Color = Color.Red;
            middleSpinningDotBoundingArea.LineThickness = 3f;

            var spinner2 = new Spinner();
            spinner.AddChild(spinner2);
            spinner2.LocalPosition -= new Vector2(2f, 0f);
            var spriteRenderer4 = new SpriteRenderer();
            spinner2.AddChild(spriteRenderer4);
            spriteRenderer4.DrawOrder = 100;
            spriteRenderer4.Sprite = new Sprite("WhiteSquare", this.Content, Point.Zero, new Point(32, 32));
            spriteRenderer4.OffsetType = OffsetType.Center;
            var outwardSpinningDotBoundingArea = new BoundingAreaDrawer();
            spriteRenderer4.AddChild(outwardSpinningDotBoundingArea);
            outwardSpinningDotBoundingArea.Color = Color.Red;
            outwardSpinningDotBoundingArea.LineThickness = 3f;

            var textRenderer = new TextRenderer();
            scene.AddChild(textRenderer);
            textRenderer.Text = "Hello, World";
            textRenderer.Font = new Font() { ContentPath = "Arial" };
            textRenderer.Color = Color.Pink;
            textRenderer.LocalScale = new Vector2(2f, 2f);
            textRenderer.LocalPosition -= new Vector2(5f, 5f);
            var textRendererBoundingArea = new BoundingAreaDrawer();
            textRendererBoundingArea.Color = Color.Red;
            textRendererBoundingArea.LineThickness = 3f;
            textRenderer.AddChild(textRendererBoundingArea);

            scene.AddChild(new MouseClickDebugger());

            scene.SaveAsJson(@"TestGame - CurrentLevel.json", new Serializer());
            this.CurrentScene = Scene.LoadFromJson(@"TestGame - CurrentLevel.json", new Serializer());

            var arrowSprite1 = PrimitiveDrawer.CreateArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
            var arrowSpriteRenderer1 = new SpriteRenderer();
            arrowSpriteRenderer1.Sprite = arrowSprite1;
            arrowSpriteRenderer1.LocalPosition += new Vector2(2f, -2f);
            this.CurrentScene.AddChild(arrowSpriteRenderer1);

            var arrowSprite2 = PrimitiveDrawer.CreateArrowSprite(this.GraphicsDevice, 32);
            var arrowSpriteRenderer2 = new SpriteRenderer();
            arrowSpriteRenderer2.Color = Color.LawnGreen;
            arrowSpriteRenderer2.Sprite = arrowSprite2;
            arrowSpriteRenderer2.LocalPosition += new Vector2(3f, -1f);
            arrowSpriteRenderer2.LocalScale = new Vector2(0.75f, 2f);
            arrowSpriteRenderer2.LocalRotation.Angle = 1f;
            this.CurrentScene.AddChild(arrowSpriteRenderer2);
        }
    }
}