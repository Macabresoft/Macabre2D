namespace Macabre2D.Examples.RenderingTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class RenderingTestGame : MacabreGame {

        protected override void Initialize() {
            base.Initialize();

            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);
            var arialId = Guid.NewGuid();
            this.AssetManager.SetMapping(arialId, "Arial");
            var coloredSquaresId = Guid.NewGuid();
            this.AssetManager.SetMapping(coloredSquaresId, "ColoredSquares");
            var whiteSquareId = Guid.NewGuid();
            this.AssetManager.SetMapping(whiteSquareId, "WhiteSquare");

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new Scene();

            var cameraScroller = new CameraScroller();
            scene.AddComponent(cameraScroller);

            var camera = new Camera();
            cameraScroller.AddChild(camera);

            var movingDot = new MovingDot();
            camera.AddChild(movingDot);
            var spriteRenderer = new SpriteRenderer();
            spriteRenderer.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer.Offset.Type = PixelOffsetType.Center;
            movingDot.AddChild(spriteRenderer);

            var spriteAnimation = new SpriteAnimation();
            var step = spriteAnimation.AddStep();
            step.Sprite = new Sprite(coloredSquaresId, Point.Zero, new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite(coloredSquaresId, new Point(0, 64), new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite(coloredSquaresId, new Point(64, 64), new Point(64, 64));
            step.Frames = 2;
            step = spriteAnimation.AddStep();
            step.Sprite = new Sprite(coloredSquaresId, new Point(64, 0), new Point(64, 64));
            step.Frames = 2;

            var spriteAnimator = new SpriteAnimator(spriteAnimation) {
                FrameRate = 4
            };

            var spriteRenderer2 = spriteAnimator.AddChild<SpriteRenderer>();
            spriteRenderer2.DrawOrder = -100;
            spriteRenderer2.Offset.Type = PixelOffsetType.Center;
            scene.AddComponent(spriteAnimator);

            var spinner = new Scaler();
            spinner.LocalPosition -= new Vector2(2f, 0);
            scene.AddComponent(spinner);
            var spriteRenderer3 = new SpriteRenderer();
            spinner.AddChild(spriteRenderer3);
            spriteRenderer3.DrawOrder = -200;
            spriteRenderer3.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer3.Offset.Type = PixelOffsetType.Center;

            var middleSpinningDotBoundingArea = new BoundingAreaDrawer();
            spriteRenderer3.AddChild(middleSpinningDotBoundingArea);
            middleSpinningDotBoundingArea.Color = Color.Red;
            middleSpinningDotBoundingArea.LineThickness = 3f;

            var spinner2 = new Scaler();
            spinner.AddChild(spinner2);
            spinner2.LocalPosition -= new Vector2(2f, 0f);
            var spriteRenderer4 = new SpriteRenderer();
            spinner2.AddChild(spriteRenderer4);
            spriteRenderer4.DrawOrder = 100;
            spriteRenderer4.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer4.Offset.Type = PixelOffsetType.Center;
            var outwardSpinningDotBoundingArea = new BoundingAreaDrawer();
            spriteRenderer4.AddChild(outwardSpinningDotBoundingArea);
            outwardSpinningDotBoundingArea.Color = Color.Red;
            outwardSpinningDotBoundingArea.LineThickness = 3f;

            var textRenderer = new TextRenderer();
            scene.AddComponent(textRenderer);
            textRenderer.Text = "Hello, World";
            textRenderer.Font = new Font(arialId);
            textRenderer.Color = Color.Pink;
            textRenderer.LocalScale = new Vector2(2f, 2f);
            textRenderer.LocalPosition -= new Vector2(5f, 5f);
            var textRendererBoundingArea = new BoundingAreaDrawer();
            textRendererBoundingArea.Color = Color.Red;
            textRendererBoundingArea.LineThickness = 3f;
            textRenderer.AddChild(textRendererBoundingArea);

            scene.AddComponent(new MouseClickDebugger());

            scene.SaveToFile(@"TestGame - CurrentLevel.json", new Serializer());
            this.CurrentScene = new Serializer().Deserialize<Scene>(@"TestGame - CurrentLevel.json");

            var arrowSprite1 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
            var arrowSpriteRenderer1 = new SpriteRenderer();
            arrowSpriteRenderer1.Sprite = arrowSprite1;
            arrowSpriteRenderer1.LocalPosition += new Vector2(2f, -2f);
            this.CurrentScene.AddComponent(arrowSpriteRenderer1);

            var arrowSprite2 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32);
            var arrowSpriteRenderer2 = new SpriteRenderer();
            arrowSpriteRenderer2.Color = Color.LawnGreen;
            arrowSpriteRenderer2.Sprite = arrowSprite2;
            arrowSpriteRenderer2.LocalPosition += new Vector2(3f, -1f);
            arrowSpriteRenderer2.LocalScale = new Vector2(0.75f, 2f);
            this.CurrentScene.AddComponent(arrowSpriteRenderer2);

            var quadSprite1 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 32), Color.Magenta);
            var quadSpriteRenderer1 = new SpriteRenderer();
            quadSpriteRenderer1.Sprite = quadSprite1;
            quadSpriteRenderer1.LocalPosition += new Vector2(3f, 2f);
            this.CurrentScene.AddComponent(quadSpriteRenderer1);

            var quadSprite2 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 64));
            var quadSpriteRenderer2 = new SpriteRenderer();
            quadSpriteRenderer2.Color = Color.Khaki;
            quadSpriteRenderer2.Sprite = quadSprite2;
            quadSpriteRenderer2.LocalPosition += new Vector2(3f, 1f);
            this.CurrentScene.AddComponent(quadSpriteRenderer2);

            var rightTriangleSprite1 = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.GraphicsDevice, new Point(32, 32), Color.MediumVioletRed);
            var rightTriangleSpriteRenderer1 = new SpriteRenderer();
            rightTriangleSpriteRenderer1.Sprite = rightTriangleSprite1;
            rightTriangleSpriteRenderer1.LocalPosition = new Vector2(-3f, 3f);
            this.CurrentScene.AddComponent(rightTriangleSpriteRenderer1);

            var circleSprite = PrimitiveDrawer.CreateCircleSprite(this.GraphicsDevice, 64, Color.Red);
            var circleSpriteRenderer = new SpriteRenderer();
            circleSpriteRenderer.Sprite = circleSprite;
            circleSpriteRenderer.LocalPosition = new Vector2(-5f, 3f);
            this.CurrentScene.AddComponent(circleSpriteRenderer);

            var binaryTileMap = new BinaryTileMapComponent {
                DrawOrder = -300,
                LocalPosition = new Vector2(-5f, -10f),
                LocalScale = new Vector2(1f, 1f),
                Sprite = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(64, 64)),
                Grid = new TileGrid(new Vector2(32, 64) * GameSettings.Instance.InversePixelsPerUnit, Vector2.Zero),
                Color = Color.DarkGray
            };

            for (var x = 0; x < 5; x++) {
                for (var y = 0; y < 10; y++) {
                    if ((x + y) % 2 == 0) {
                        binaryTileMap.AddTile(new Point(x, y));
                    }
                }
            }

            var binaryTileMapBoundingArea = new BoundingAreaDrawer();
            binaryTileMap.AddChild(binaryTileMapBoundingArea);
            binaryTileMapBoundingArea.Color = Color.Red;
            binaryTileMapBoundingArea.LineThickness = 3f;

            this.CurrentScene.AddComponent(binaryTileMap);

            this._isLoaded = true;
        }
    }
}