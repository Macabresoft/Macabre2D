namespace Macabresoft.Macabre2D.Samples.Content {
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [ExcludeFromCodeCoverage]
    public class ContentGame : BaseGame {
        protected override void LoadContent() {
            this.Project.Settings.PixelsPerUnit = 64;

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new GameScene();

            scene.AddSystem<UpdateSystem>();
            scene.AddSystem<RenderSystem>();

            var cameraEntity = scene.AddChild();
            cameraEntity.AddComponent<CameraScroller>();
            var camera = cameraEntity.AddComponent<CameraComponent>();
            camera.LayersToRender = Layers.Default;
            camera.OffsetSettings.OffsetType = PixelOffsetType.Center;
            cameraEntity.AddComponent<MovingDot>();
            cameraEntity.AddChild().AddComponent<MouseClickDebugger>();

            var whiteSquare = new SpriteSheet();
            this.Assets.RegisterMetadata(new ContentMetadata(whiteSquare, new [] { "WhiteSquare" }, ".png"));

            var spriteRenderer = cameraEntity.AddComponent<SpriteRenderComponent>();
            spriteRenderer.SpriteReference.Initialize(whiteSquare);
            spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;

            var binaryTileMapEntity = scene.AddChild();
            var gridComponent = binaryTileMapEntity.AddComponent<GridComponent>();
            gridComponent.Grid = new TileGrid(new Vector2(32, 64) * this.Project.Settings.InversePixelsPerUnit);
            var binaryTileMap = binaryTileMapEntity.AddComponent<BinaryTileMapComponent>();
            binaryTileMap.RenderOrder = -300;
            binaryTileMapEntity.LocalPosition = new Vector2(-5f, -10f);
            binaryTileMapEntity.LocalScale = new Vector2(1f, 1f);
            binaryTileMap.SpriteReference.Initialize(whiteSquare);
            binaryTileMap.Color = Color.DarkGray;

            for (var x = 0; x < 5; x++) {
                for (var y = 0; y < 10; y++) {
                    if ((x + y) % 2 == 0) {
                        binaryTileMap.AddTile(new Point(x, y));
                    }
                }
            }

            this.PreLoadAudioStuff(scene);

            var font = new Font();
            this.Assets.RegisterMetadata(new ContentMetadata(font, new[] { "League Mono" }, ".spritefont"));

            var colorfulSquares = new SpriteSheet {
                Rows = 2,
                Columns = 2
            };

            this.Assets.RegisterMetadata(new ContentMetadata(colorfulSquares, new[] { "ColorfulSquares" }, ".png"));

            var animatedEntity = scene.AddChild();
            var spriteAnimator = animatedEntity.AddComponent<SpriteAnimatorComponent>();
            spriteAnimator.FrameRate = 4;
            spriteAnimator.RenderOrder = -100;
            spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

            var spriteAnimation = new SpriteAnimation();
            for (byte i = 0; i < 4; i++) {
                var step = spriteAnimation.AddStep();
                step.SpriteIndex = i;
                step.Frames = 2;
            }

            colorfulSquares.AddPackage(spriteAnimation);
            spriteAnimator.AnimationReference.PackagedAssetId = spriteAnimation.Id;
            spriteAnimator.AnimationReference.Initialize(colorfulSquares);

            var scalerEntity1 = scene.AddChild();
            scalerEntity1.AddComponent<Scaler>();
            scalerEntity1.LocalPosition -= new Vector2(2f, 0);
            var spriteRenderer3 = scalerEntity1.AddComponent<SpriteRenderComponent>();
            spriteRenderer3.RenderOrder = -200;
            spriteRenderer3.SpriteReference.Initialize(whiteSquare);
            spriteRenderer3.RenderSettings.OffsetType = PixelOffsetType.Center;
            var middleSpinningDotBoundingArea = scalerEntity1.AddComponent<BoundingAreaDrawerComponent>();
            middleSpinningDotBoundingArea.Color = Color.Red;
            middleSpinningDotBoundingArea.LineThickness = 3f;

            var scalerEntity2 = scalerEntity1.AddChild();
            scalerEntity2.AddComponent<Scaler>();
            scalerEntity2.LocalPosition -= new Vector2(2f, 0f);
            var spriteRenderer4 = scalerEntity2.AddComponent<SpriteRenderComponent>();
            spriteRenderer4.RenderOrder = 100;
            spriteRenderer4.SpriteReference.Initialize(whiteSquare);
            spriteRenderer4.RenderSettings.OffsetType = PixelOffsetType.Center;
            var outwardSpinningDotBoundingArea = scalerEntity2.AddComponent<BoundingAreaDrawerComponent>();
            outwardSpinningDotBoundingArea.Color = Color.Red;
            outwardSpinningDotBoundingArea.LineThickness = 3f;

            var textEntity = scene.AddChild();
            var textRenderer = textEntity.AddComponent<TextRenderComponent>();
            textRenderer.Text = "Hello, World";
            textRenderer.FontReference.Initialize(font);
            textRenderer.Color = Color.DarkMagenta;
            textEntity.LocalScale = new Vector2(0.5f, 0.5f);
            textEntity.LocalPosition -= new Vector2(5f, 5f);
            var textRendererBoundingArea = textEntity.AddComponent<BoundingAreaDrawerComponent>();
            textRendererBoundingArea.Color = Color.Red;
            textRendererBoundingArea.LineThickness = 3f;

            var secondCameraEntity = scene.AddChild();
            var secondCamera = secondCameraEntity.AddComponent<CameraComponent>();
            secondCamera.LayersToRender = Layers.Layer03;
            var frameRateDisplayEntity = secondCameraEntity.AddChild();
            frameRateDisplayEntity.Layers = Layers.Layer03;
            var frameRateDisplay = frameRateDisplayEntity.AddComponent<FrameRateDisplayComponent>();
            frameRateDisplay.FontReference.Initialize(font);
            frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
            frameRateDisplayEntity.LocalScale = new Vector2(0.1f);

            scene.Initialize(this);

            var filePath = Path.GetTempFileName();
            Serializer.Instance.Serialize(scene, filePath);
            scene = Serializer.Instance.Deserialize<GameScene>(filePath);
            File.Delete(filePath);

            this.LoadScene(scene);
            this.PostLoadRenderingStuff();
        }

        private void PostLoadRenderingStuff() {
            var arrowSprite1 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
            var arrowSpriteEntity1 = this.Scene.AddChild();
            var arrowSpriteRenderer1 = arrowSpriteEntity1.AddComponent<Texture2DRenderComponent>();
            arrowSpriteRenderer1.Texture = arrowSprite1;
            arrowSpriteEntity1.LocalPosition += new Vector2(2f, -2f);

            var arrowSprite2 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32);
            var arrowSpriteEntity2 = this.Scene.AddChild();
            var arrowSpriteRenderer2 = arrowSpriteEntity2.AddComponent<Texture2DRenderComponent>();
            arrowSpriteRenderer2.Color = Color.LawnGreen;
            arrowSpriteRenderer2.Texture = arrowSprite2;
            arrowSpriteEntity2.LocalPosition += new Vector2(3f, -1f);
            arrowSpriteEntity2.LocalScale = new Vector2(0.75f, 2f);

            var quadSprite1 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 32), Color.Magenta);
            var quadEntity1 = this.Scene.AddChild();
            var quadSpriteRenderer1 = quadEntity1.AddComponent<Texture2DRenderComponent>();
            quadSpriteRenderer1.Texture = quadSprite1;
            quadEntity1.LocalPosition += new Vector2(3f, 2f);

            var quadSprite2 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 64));
            var quadEntity2 = this.Scene.AddChild();
            var quadSpriteRenderer2 = quadEntity2.AddComponent<Texture2DRenderComponent>();
            quadSpriteRenderer2.Color = Color.Khaki;
            quadSpriteRenderer2.Texture = quadSprite2;
            quadEntity2.LocalPosition += new Vector2(3f, 1f);

            var rightTriangleSprite1 = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.GraphicsDevice, new Point(32, 32), Color.MediumVioletRed);
            var rightTriangleEntity = this.Scene.AddChild();
            var rightTriangleSpriteRenderer1 = rightTriangleEntity.AddComponent<Texture2DRenderComponent>();
            rightTriangleSpriteRenderer1.Texture = rightTriangleSprite1;
            rightTriangleEntity.LocalPosition = new Vector2(-3f, 3f);

            var circleSprite = PrimitiveDrawer.CreateCircleSprite(this.GraphicsDevice, 64, Color.Red);
            var circleEntity = this.Scene.AddChild();
            var circleSpriteRenderer = circleEntity.AddComponent<Texture2DRenderComponent>();
            circleSpriteRenderer.Texture = circleSprite;
            circleEntity.LocalPosition = new Vector2(-5f, 3f);

            var gridDrawer = this.Scene.AddChild().AddComponent<GridDrawerComponent>();
            gridDrawer.Color = DefinedColors.MacabresoftBone * 0.5f;
            gridDrawer.UseDynamicLineThickness = true;
            gridDrawer.Grid = new TileGrid(Vector2.One);
            gridDrawer.RenderOrder = -1;

            if (this.Scene.TryGetComponent<BinaryTileMapComponent>(out var binaryTileMapComponent) && binaryTileMapComponent != null) {
                var binaryTileMapBoundingArea = binaryTileMapComponent.Entity.AddComponent<BoundingAreaDrawerComponent>();
                binaryTileMapBoundingArea.Color = Color.Red;
                binaryTileMapBoundingArea.LineThickness = 3f;
            }
        }

        private void PreLoadAudioStuff(GameScene scene) {
            var laser = new AudioClip();
            this.Assets.RegisterMetadata(new ContentMetadata(laser, new[] { "laser" }, ".wav"));

            var audioEntity = scene.AddChild();
            var audioPlayer = audioEntity.AddComponent<AudioPlayerComponent>();
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClipReference.Initialize(laser);
            audioEntity.AddComponent<VolumeController>();
        }
    }
}