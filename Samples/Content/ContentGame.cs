namespace Macabresoft.Macabre2D.Samples.Content;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

[ExcludeFromCodeCoverage]
public class ContentGame : BaseGame {
    private SpriteSheetAsset _colorfulSquares;
    private FontAsset _font;
    private AudioClipAsset _laser;
    private PrefabAsset _prefabHelloWorld;

    private SpriteSheetAsset _whiteSquare;

    protected override void LoadContent() {
        this.TryCreateSpriteBatch();
        this.Project.Settings.PixelsPerUnit = 64;

        var scene = new Scene();

        scene.AddLoop<UpdateLoop>();
        scene.AddLoop<RenderLoop>();

        var camera = scene.AddChild<Camera>();
        camera.AddChild<CameraScroller>();
        camera.LayersToRender = Layers.Default;
        camera.OffsetSettings.OffsetType = PixelOffsetType.Center;
        camera.AddChild<MovingDot>();
        camera.AddChild<MouseClickDebugger>();

        this._whiteSquare = new SpriteSheetAsset();

        var spriteRenderer = camera.AddChild<SpriteRenderer>();
        spriteRenderer.SpriteReference.LoadAsset(this._whiteSquare);
        spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;

        var gridContainer = scene.AddChild<GridContainer>();
        gridContainer.LocalTileSize = new Vector2(2, 4);
        var binaryTileMap = gridContainer.AddChild<BinaryTileMap>();
        binaryTileMap.RenderOrder = -300;
        binaryTileMap.LocalPosition = new Vector2(-5f, -10f);
        binaryTileMap.LocalScale = new Vector2(1f, 1f);
        binaryTileMap.SpriteReference.LoadAsset(this._whiteSquare);
        binaryTileMap.Color = Color.DarkGray;

        for (var x = 0; x < 5; x++) {
            for (var y = 0; y < 10; y++) {
                if ((x + y) % 2 == 0) {
                    binaryTileMap.AddTile(new Point(x, y));
                }
            }
        }

        this._font = new FontAsset();

        this._colorfulSquares = new SpriteSheetAsset {
            Rows = 2,
            Columns = 2
        };

        var spriteAnimator = scene.AddChild<LoopingSpriteAnimator>();
        spriteAnimator.FrameRate = 4;
        spriteAnimator.RenderOrder = -100;
        spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this._colorfulSquares.SpriteAnimations is SpriteAnimationCollection spriteAnimations) {
            var spriteAnimation = new SpriteAnimation();
            for (byte i = 0; i < 4; i++) {
                var step = spriteAnimation.AddStep();
                step.SpriteIndex = i;
                step.Frames = 2;
            }

            spriteAnimations.Add(spriteAnimation);
            spriteAnimator.AnimationReference.PackagedAssetId = spriteAnimation.Id;
            spriteAnimator.AnimationReference.LoadAsset(this._colorfulSquares);
        }

        var spriteRenderer3 = scene.AddChild<SpriteRenderer>();
        spriteRenderer3.RenderOrder = -200;
        spriteRenderer3.SpriteReference.LoadAsset(this._whiteSquare);
        spriteRenderer3.RenderSettings.OffsetType = PixelOffsetType.Center;
        spriteRenderer3.LocalPosition -= new Vector2(2f, 0);
        spriteRenderer3.AddChild<Scaler>();
        var middleSpinningDotBoundingArea = spriteRenderer3.AddChild<BoundingAreaDrawer>();
        middleSpinningDotBoundingArea.Color = Color.Red;
        middleSpinningDotBoundingArea.LineThickness = 3f;

        var spriteRenderer4 = spriteRenderer3.AddChild<SpriteRenderer>();
        spriteRenderer4.RenderOrder = 100;
        spriteRenderer4.SpriteReference.LoadAsset(this._whiteSquare);
        spriteRenderer4.RenderSettings.OffsetType = PixelOffsetType.Center;
        spriteRenderer4.LocalPosition -= new Vector2(2f, 0f);
        spriteRenderer4.AddChild<Scaler>();
        var outwardSpinningDotBoundingArea = spriteRenderer4.AddChild<BoundingAreaDrawer>();
        outwardSpinningDotBoundingArea.Color = Color.Red;
        outwardSpinningDotBoundingArea.LineThickness = 3f;

        var textRenderer = new TextRenderer();
        textRenderer.Text = "Hello, World";
        textRenderer.FontReference.LoadAsset(this._font);
        textRenderer.Color = Color.DarkMagenta;
        var textRendererBoundingArea = textRenderer.AddChild<BoundingAreaDrawer>();
        textRendererBoundingArea.Color = Color.Red;
        textRendererBoundingArea.LineThickness = 3f;

        this._prefabHelloWorld = new PrefabAsset();
        this._prefabHelloWorld.LoadContent(textRenderer);

        var prefabContainer1 = scene.AddChild<PrefabContainer>();
        prefabContainer1.LocalScale = new Vector2(0.5f, 0.5f);
        prefabContainer1.LocalPosition -= new Vector2(5f, 5f);
        prefabContainer1.PrefabReference.LoadAsset(this._prefabHelloWorld);

        var prefabContainer2 = scene.AddChild<PrefabContainer>();
        prefabContainer2.LocalScale = new Vector2(2f, 2f);
        prefabContainer2.LocalPosition = new Vector2(-6f, 3f);
        prefabContainer2.PrefabReference.LoadAsset(this._prefabHelloWorld);

        var secondCamera = scene.AddChild<Camera>();
        secondCamera.LayersToRender = Layers.Layer03;
        var frameRateDisplay = secondCamera.AddChild<FrameRateDisplayEntity>();
        frameRateDisplay.Layers = Layers.Layer03;
        frameRateDisplay.FontReference.LoadAsset(this._font);
        frameRateDisplay.Color = DefinedColors.ZvukostiGreen;
        frameRateDisplay.LocalScale = new Vector2(0.1f);

        this._laser = new AudioClipAsset();
        var audioPlayer = scene.AddChild<AudioPlayer>();
        audioPlayer.Volume = 0.5f;
        audioPlayer.AudioClipReference.LoadAsset(this._laser);
        audioPlayer.AddChild<VolumeController>();

        var filePath = Path.GetTempFileName();
        Serializer.Instance.Serialize(scene, filePath);
        scene = Serializer.Instance.Deserialize<Scene>(filePath);
        File.Delete(filePath);

        this.LoadScene(scene);
        this.PostLoadRenderingStuff();
    }

    protected override void RegisterNewSceneMetadata(IAssetManager assetManager) {
        base.RegisterNewSceneMetadata(assetManager);

        assetManager.RegisterMetadata(new ContentMetadata(this._colorfulSquares, new[] { "ColorfulSquares" }, ".png"));
        assetManager.RegisterMetadata(new ContentMetadata(this._whiteSquare, new[] { "WhiteSquare" }, ".png"));
        assetManager.RegisterMetadata(new ContentMetadata(this._font, new[] { "League Mono" }, ".spritefont"));
        assetManager.RegisterMetadata(new ContentMetadata(this._laser, new[] { "laser" }, ".wav"));
        assetManager.RegisterMetadata(new ContentMetadata(this._prefabHelloWorld, new[] { "PrefabHelloWorld" }, PrefabAsset.FileExtension));
    }

    private void PostLoadRenderingStuff() {
        var arrowSprite1 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32, Color.Goldenrod);
        var arrowSpriteRenderer1 = this.Scene.AddChild<Texture2DRenderer>();
        arrowSpriteRenderer1.Texture = arrowSprite1;
        arrowSpriteRenderer1.LocalPosition += new Vector2(2f, -2f);

        var arrowSprite2 = PrimitiveDrawer.CreateUpwardsArrowSprite(this.GraphicsDevice, 32);
        var arrowSpriteRenderer2 = this.Scene.AddChild<Texture2DRenderer>();
        arrowSpriteRenderer2.Color = Color.LawnGreen;
        arrowSpriteRenderer2.Texture = arrowSprite2;
        arrowSpriteRenderer2.LocalPosition += new Vector2(3f, -1f);
        arrowSpriteRenderer2.LocalScale = new Vector2(0.75f, 2f);

        var quadSprite1 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 32), Color.Magenta);
        var quadSpriteRenderer1 = this.Scene.AddChild<Texture2DRenderer>();
        quadSpriteRenderer1.Texture = quadSprite1;
        quadSpriteRenderer1.LocalPosition += new Vector2(3f, 2f);

        var quadSprite2 = PrimitiveDrawer.CreateQuadSprite(this.GraphicsDevice, new Point(32, 64));
        var quadSpriteRenderer2 = this.Scene.AddChild<Texture2DRenderer>();
        quadSpriteRenderer2.Color = Color.Khaki;
        quadSpriteRenderer2.Texture = quadSprite2;
        quadSpriteRenderer2.LocalPosition += new Vector2(3f, 1f);

        var rightTriangleSprite1 = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.GraphicsDevice, new Point(32, 32), Color.MediumVioletRed);
        var rightTriangleSpriteRenderer1 = this.Scene.AddChild<Texture2DRenderer>();
        rightTriangleSpriteRenderer1.Texture = rightTriangleSprite1;
        rightTriangleSpriteRenderer1.LocalPosition = new Vector2(-3f, 3f);

        var circleSprite = PrimitiveDrawer.CreateCircleSprite(this.GraphicsDevice, 64, Color.Red);
        var circleSpriteRenderer = this.Scene.AddChild<Texture2DRenderer>();
        circleSpriteRenderer.Texture = circleSprite;
        circleSpriteRenderer.LocalPosition = new Vector2(-5f, 3f);

        if (this.Scene.TryGetChild<BinaryTileMap>(out var binaryTileMap) && binaryTileMap != null) {
            var binaryTileMapBoundingArea = binaryTileMap.AddChild<BoundingAreaDrawer>();
            binaryTileMapBoundingArea.Color = Color.Red;
            binaryTileMapBoundingArea.LineThickness = 3f;
        }
    }
}