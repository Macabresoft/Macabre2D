namespace Macabre2D.Examples.MultiPlatformTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    public class MultiPlatformGame : MacabreGame {

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new Scene();

            var cameraScroller = new CameraScroller();
            scene.AddChild(cameraScroller);

            var camera = new Camera();
            camera.OffsetSettings.OffsetType = PixelOffsetType.Center;
            cameraScroller.AddChild(camera);

            var movingDot = new MovingDot();
            camera.AddChild(movingDot);

            this.AssetManager.SetMapping(Guid.NewGuid(), "WhiteSquare");

            var spriteRenderer = new SpriteRenderComponent();
            spriteRenderer.Sprite = new Sprite(this.AssetManager.GetId("WhiteSquare"), Point.Zero, new Point(32, 32));
            spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            movingDot.AddChild(spriteRenderer);

            this.PreLoadAudioStuff(scene);
            this.PreLoadRenderingStuff(scene);

            scene.SaveToFile(@"TestGame - CurrentLevel.json");
            this.CurrentScene = Serializer.Instance.Deserialize<Scene>(@"TestGame - CurrentLevel.json");

            this.PostLoadAudioStuff();
            this.PostLoadRenderingStuff();

            this._isLoaded = true;
        }

        private void PostLoadAudioStuff() {
            var pianoComponent = this.CurrentScene.AddChild<PianoComponent>();
            pianoComponent.LocalPosition -= new Vector2(6f, 15f);
        }

        private void PostLoadRenderingStuff() {
            var arrowSprite1 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
            var arrowSpriteRenderer1 = new SpriteRenderComponent();
            arrowSpriteRenderer1.Sprite = arrowSprite1;
            arrowSpriteRenderer1.LocalPosition += new Vector2(2f, -2f);
            this.CurrentScene.AddChild(arrowSpriteRenderer1);

            var arrowSprite2 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32);
            var arrowSpriteRenderer2 = new SpriteRenderComponent();
            arrowSpriteRenderer2.Color = Color.LawnGreen;
            arrowSpriteRenderer2.Sprite = arrowSprite2;
            arrowSpriteRenderer2.LocalPosition += new Vector2(3f, -1f);
            arrowSpriteRenderer2.LocalScale = new Vector2(0.75f, 2f);
            this.CurrentScene.AddChild(arrowSpriteRenderer2);

            var quadSprite1 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 32), Color.Magenta);
            var quadSpriteRenderer1 = new SpriteRenderComponent();
            quadSpriteRenderer1.Sprite = quadSprite1;
            quadSpriteRenderer1.LocalPosition += new Vector2(3f, 2f);
            this.CurrentScene.AddChild(quadSpriteRenderer1);

            var quadSprite2 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 64));
            var quadSpriteRenderer2 = new SpriteRenderComponent();
            quadSpriteRenderer2.Color = Color.Khaki;
            quadSpriteRenderer2.Sprite = quadSprite2;
            quadSpriteRenderer2.LocalPosition += new Vector2(3f, 1f);
            this.CurrentScene.AddChild(quadSpriteRenderer2);

            var rightTriangleSprite1 = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.GraphicsDevice, new Point(32, 32), Color.MediumVioletRed);
            var rightTriangleSpriteRenderer1 = new SpriteRenderComponent();
            rightTriangleSpriteRenderer1.Sprite = rightTriangleSprite1;
            rightTriangleSpriteRenderer1.LocalPosition = new Vector2(-3f, 3f);
            this.CurrentScene.AddChild(rightTriangleSpriteRenderer1);

            var circleSprite = PrimitiveDrawer.CreateCircleSprite(this.GraphicsDevice, 64, Color.Red);
            var circleSpriteRenderer = new SpriteRenderComponent();
            circleSpriteRenderer.Sprite = circleSprite;
            circleSpriteRenderer.LocalPosition = new Vector2(-5f, 3f);
            this.CurrentScene.AddChild(circleSpriteRenderer);

            var binaryTileMap = new BinaryTileMap {
                DrawOrder = -300,
                LocalPosition = new Vector2(-5f, -10f),
                LocalScale = new Vector2(1f, 1f),
                Sprite = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(64, 64)),
                Color = Color.DarkGray
            };

            binaryTileMap.GridConfiguration.LocalGrid = new TileGrid(new Vector2(32, 64) * GameSettings.Instance.InversePixelsPerUnit);

            for (var x = 0; x < 5; x++) {
                for (var y = 0; y < 10; y++) {
                    if ((x + y) % 2 == 0) {
                        binaryTileMap.AddTile(new Point(x, y));
                    }
                }
            }

            var binaryTileMapBoundingArea = new BoundingAreaDrawerComponent();
            binaryTileMap.AddChild(binaryTileMapBoundingArea);
            binaryTileMapBoundingArea.Color = Color.Red;
            binaryTileMapBoundingArea.LineThickness = 3f;

            this.CurrentScene.AddChild(binaryTileMap);

            var spriteAnimation = new SpriteAnimation();
            var coloredSquaresId = this.AssetManager.GetId("ColoredSquares");
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

            var spriteAnimator = this.CurrentScene.GetAllComponentsOfType<SpriteAnimationComponent>().First();
            spriteAnimator.Enqueue(spriteAnimation, true);
        }

        private void PreLoadAudioStuff(Scene scene) {
            var lasterId = Guid.NewGuid();
            this.AssetManager.SetMapping(lasterId, "laser");

            var audioPlayer = new AudioPlayer();
            scene.AddChild(audioPlayer);
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClip = new AudioClip();
            audioPlayer.AudioClip.Id = lasterId;
            audioPlayer.AddChild(new VolumeController());
        }

        private void PreLoadRenderingStuff(Scene scene) {
            var fontId = Guid.NewGuid();
            this.AssetManager.SetMapping(fontId, "League Mono");
            var coloredSquaresId = Guid.NewGuid();
            this.AssetManager.SetMapping(coloredSquaresId, "ColoredSquares");
            var whiteSquareId = Guid.NewGuid();
            this.AssetManager.SetMapping(whiteSquareId, "WhiteSquare");

            var spriteAnimator = new SpriteAnimationComponent() {
                FrameRate = 4
            };

            spriteAnimator.DrawOrder = -100;
            spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;
            scene.AddChild(spriteAnimator);

            var spinner = new Scaler();
            spinner.LocalPosition -= new Vector2(2f, 0);
            scene.AddChild(spinner);
            var spriteRenderer3 = new SpriteRenderComponent();
            spinner.AddChild(spriteRenderer3);
            spriteRenderer3.DrawOrder = -200;
            spriteRenderer3.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer3.RenderSettings.OffsetType = PixelOffsetType.Center;

            var middleSpinningDotBoundingArea = new BoundingAreaDrawerComponent();
            spriteRenderer3.AddChild(middleSpinningDotBoundingArea);
            middleSpinningDotBoundingArea.Color = Color.Red;
            middleSpinningDotBoundingArea.LineThickness = 3f;

            var spinner2 = new Scaler();
            spinner.AddChild(spinner2);
            spinner2.LocalPosition -= new Vector2(2f, 0f);
            var spriteRenderer4 = new SpriteRenderComponent();
            spinner2.AddChild(spriteRenderer4);
            spriteRenderer4.DrawOrder = 100;
            spriteRenderer4.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer4.RenderSettings.OffsetType = PixelOffsetType.Center;
            var outwardSpinningDotBoundingArea = new BoundingAreaDrawerComponent();
            spriteRenderer4.AddChild(outwardSpinningDotBoundingArea);
            outwardSpinningDotBoundingArea.Color = Color.Red;
            outwardSpinningDotBoundingArea.LineThickness = 3f;

            var textRenderer = new TextRenderComponent();
            scene.AddChild(textRenderer);
            textRenderer.Text = "Hello, World";
            textRenderer.Font = new Font(fontId);
            textRenderer.Color = Color.DarkMagenta;
            textRenderer.LocalScale = new Vector2(0.5f, 0.5f);
            textRenderer.LocalPosition -= new Vector2(5f, 5f);
            var textRendererBoundingArea = new BoundingAreaDrawerComponent();
            textRendererBoundingArea.Color = Color.Red;
            textRendererBoundingArea.LineThickness = 3f;
            textRenderer.AddChild(textRendererBoundingArea);

            scene.AddChild(new MouseClickDebugger());
        }
    }
}