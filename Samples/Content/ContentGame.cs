namespace Macabresoft.MonoGame.Samples.Content {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ContentGame : DefaultGame {

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new GameScene();

            var cameraEntity = scene.AddChild();
            cameraEntity.AddComponent<CameraScroller>();
            var camera = cameraEntity.AddComponent<CameraComponent>();
            camera.OffsetSettings.OffsetType = PixelOffsetType.Center;
            cameraEntity.AddComponent<MovingDot>();

            this.AssetManager.SetMapping(Guid.NewGuid(), "WhiteSquare");

            var spriteRenderer = cameraEntity.AddComponent<SpriteRenderComponent>();
            spriteRenderer.Sprite = new Sprite(this.AssetManager.GetId("WhiteSquare"), Point.Zero, new Point(32, 32));
            spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;

            this.PreLoadAudioStuff(scene);
            this.PreLoadRenderingStuff(scene);

            scene.Initialize(this);

            Serializer.Instance.Serialize(scene, @"Content Game - Scene.json");
            scene = Serializer.Instance.Deserialize<GameScene>(@"Content Game - Scene.json");
            this.LoadScene(scene);

            this.PostLoadRenderingStuff();
        }

        private void PostLoadRenderingStuff() {
            ////var arrowSprite1 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
            ////var arrowSpriteRenderer1 = new SpriteRenderComponent();
            ////arrowSpriteRenderer1.Sprite = arrowSprite1;
            ////arrowSpriteRenderer1.LocalPosition += new Vector2(2f, -2f);
            ////this.CurrentScene.AddChild(arrowSpriteRenderer1);

            ////var arrowSprite2 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32);
            ////var arrowSpriteRenderer2 = new SpriteRenderComponent();
            ////arrowSpriteRenderer2.Color = Color.LawnGreen;
            ////arrowSpriteRenderer2.Sprite = arrowSprite2;
            ////arrowSpriteRenderer2.LocalPosition += new Vector2(3f, -1f);
            ////arrowSpriteRenderer2.LocalScale = new Vector2(0.75f, 2f);
            ////this.CurrentScene.AddChild(arrowSpriteRenderer2);

            ////var quadSprite1 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 32), Color.Magenta);
            ////var quadSpriteRenderer1 = new SpriteRenderComponent();
            ////quadSpriteRenderer1.Sprite = quadSprite1;
            ////quadSpriteRenderer1.LocalPosition += new Vector2(3f, 2f);
            ////this.CurrentScene.AddChild(quadSpriteRenderer1);

            ////var quadSprite2 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 64));
            ////var quadSpriteRenderer2 = new SpriteRenderComponent();
            ////quadSpriteRenderer2.Color = Color.Khaki;
            ////quadSpriteRenderer2.Sprite = quadSprite2;
            ////quadSpriteRenderer2.LocalPosition += new Vector2(3f, 1f);
            ////this.CurrentScene.AddChild(quadSpriteRenderer2);

            ////var rightTriangleSprite1 = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.GraphicsDevice, new Point(32, 32), Color.MediumVioletRed);
            ////var rightTriangleSpriteRenderer1 = new SpriteRenderComponent();
            ////rightTriangleSpriteRenderer1.Sprite = rightTriangleSprite1;
            ////rightTriangleSpriteRenderer1.LocalPosition = new Vector2(-3f, 3f);
            ////this.CurrentScene.AddChild(rightTriangleSpriteRenderer1);

            ////var circleSprite = PrimitiveDrawer.CreateCircleSprite(this.GraphicsDevice, 64, Color.Red);
            ////var circleSpriteRenderer = new SpriteRenderComponent();
            ////circleSpriteRenderer.Sprite = circleSprite;
            ////circleSpriteRenderer.LocalPosition = new Vector2(-5f, 3f);
            ////this.CurrentScene.AddChild(circleSpriteRenderer);

            var binaryTileMapEntity = this.Scene.AddChild();
            var binaryTileMap = binaryTileMapEntity.AddComponent<BinaryTileMap>();
            binaryTileMap.RenderOrder = -300;
            binaryTileMapEntity.LocalPosition = new Vector2(-5f, -10f);
            binaryTileMapEntity.LocalScale = new Vector2(1f, 1f);
            binaryTileMap.Sprite = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(64, 64));
            binaryTileMap.Color = Color.DarkGray;

            binaryTileMap.Grid = new TileGrid(new Vector2(32, 64) * GameSettings.Instance.InversePixelsPerUnit);

            for (var x = 0; x < 5; x++) {
                for (var y = 0; y < 10; y++) {
                    if ((x + y) % 2 == 0) {
                        binaryTileMap.AddTile(new Point(x, y));
                    }
                }
            }

            var binaryTileMapBoundingArea = binaryTileMapEntity.AddComponent<BoundingAreaDrawerComponent>();
            binaryTileMapBoundingArea.Color = Color.Red;
            binaryTileMapBoundingArea.LineThickness = 3f;

            ////var spriteAnimation = new SpriteAnimation();
            ////var coloredSquaresId = this.AssetManager.GetId("ColoredSquares");
            ////var step = spriteAnimation.AddStep();
            ////step.Sprite = new Sprite(coloredSquaresId, Point.Zero, new Point(64, 64));
            ////step.Frames = 2;
            ////step = spriteAnimation.AddStep();
            ////step.Sprite = new Sprite(coloredSquaresId, new Point(0, 64), new Point(64, 64));
            ////step.Frames = 2;
            ////step = spriteAnimation.AddStep();
            ////step.Sprite = new Sprite(coloredSquaresId, new Point(64, 64), new Point(64, 64));
            ////step.Frames = 2;
            ////step = spriteAnimation.AddStep();
            ////step.Sprite = new Sprite(coloredSquaresId, new Point(64, 0), new Point(64, 64));
            ////step.Frames = 2;

            ////var spriteAnimator = this.Scene.GetAllComponentsOfType<SpriteAnimationComponent>().First();
            ////spriteAnimator.Enqueue(spriteAnimation, true);
        }

        private void PreLoadAudioStuff(GameScene scene) {
            var lasterId = Guid.NewGuid();
            this.AssetManager.SetMapping(lasterId, "laser");

            var audioEntity = scene.AddChild();
            var audioPlayer = audioEntity.AddComponent<AudioPlayerComponent>();
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClip = new AudioClip();
            audioPlayer.AudioClip.Id = lasterId;
            audioEntity.AddComponent<VolumeController>();
        }

        private void PreLoadRenderingStuff(GameScene scene) {
            var fontId = Guid.NewGuid();
            this.AssetManager.SetMapping(fontId, "League Mono");
            var coloredSquaresId = Guid.NewGuid();
            this.AssetManager.SetMapping(coloredSquaresId, "ColoredSquares");
            var whiteSquareId = Guid.NewGuid();
            this.AssetManager.SetMapping(whiteSquareId, "WhiteSquare");

            var animatedEntity = scene.AddChild();
            var spriteAnimator = animatedEntity.AddComponent<SpriteAnimationComponent>();
            spriteAnimator.FrameRate = 4;
            spriteAnimator.RenderOrder = -100;
            spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

            var scalerEntity1 = scene.AddChild();
            scalerEntity1.AddComponent<Scaler>();
            scalerEntity1.LocalPosition -= new Vector2(2f, 0);
            var spriteRenderer3 = scalerEntity1.AddComponent<SpriteRenderComponent>();
            spriteRenderer3.RenderOrder = -200;
            spriteRenderer3.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer3.RenderSettings.OffsetType = PixelOffsetType.Center;
            var middleSpinningDotBoundingArea = scalerEntity1.AddComponent<BoundingAreaDrawerComponent>();
            middleSpinningDotBoundingArea.Color = Color.Red;
            middleSpinningDotBoundingArea.LineThickness = 3f;

            var scalerEntity2 = scene.AddChild();
            scalerEntity2.AddComponent<Scaler>();
            scalerEntity2.LocalPosition -= new Vector2(2f, 0f);
            var spriteRenderer4 = scalerEntity2.AddComponent<SpriteRenderComponent>();
            spriteRenderer4.RenderOrder = 100;
            spriteRenderer4.Sprite = new Sprite(whiteSquareId, Point.Zero, new Point(32, 32));
            spriteRenderer4.RenderSettings.OffsetType = PixelOffsetType.Center;
            var outwardSpinningDotBoundingArea = scalerEntity2.AddComponent<BoundingAreaDrawerComponent>();
            outwardSpinningDotBoundingArea.Color = Color.Red;
            outwardSpinningDotBoundingArea.LineThickness = 3f;

            var textEntity = scene.AddChild();
            var textRenderer = textEntity.AddComponent<TextRenderComponent>();
            textRenderer.Text = "Hello, World";
            textRenderer.Font = new Font(fontId);
            textRenderer.Color = Color.DarkMagenta;
            textEntity.LocalScale = new Vector2(0.5f, 0.5f);
            textEntity.LocalPosition -= new Vector2(5f, 5f);
            var textRendererBoundingArea = textEntity.AddComponent<BoundingAreaDrawerComponent>();
            textRendererBoundingArea.Color = Color.Red;
            textRendererBoundingArea.LineThickness = 3f;

            var mouseClickEntity = scene.AddChild();
            mouseClickEntity.AddComponent<MouseClickDebugger>();
        }
    }
}