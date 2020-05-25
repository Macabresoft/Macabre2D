namespace Macabre2D.Examples.SynthTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class SynthGame : MacabreGame {
        private const string PianoSpriteSheetName = "PianoRollSpriteSheet";
        private static readonly Point BlackPressedKeySpriteLocation = new Point(0, 32);
        private static readonly Point BlackUnpressedKeySpriteLocation = new Point(0, 0);
        private static readonly Point PianoKeySpriteSize = new Point(32, 16);
        private static readonly Point WhitePressedKeySpriteLocation = new Point(0, 48);
        private static readonly Point WhiteUnpressedKeySpriteLocation = new Point(0, 16);

        protected override void Initialize() {
            base.Initialize();

            this._graphics.IsFullScreen = false;
            this._graphics.PreferredBackBufferHeight = 768;
            this._graphics.PreferredBackBufferWidth = 1024;
            this._graphics.ApplyChanges();
            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.CurrentScene = new Scene {
                BackgroundColor = Color.Teal
            };

            var camera = this.CurrentScene.AddChild<Camera>();
            camera.ViewHeight = 36f;
            camera.OffsetSettings.OffsetType = PixelOffsetType.BottomLeft;
            camera.LocalPosition = Vector2.Zero;

            if (this.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 16;
            }

            var spriteSheetId = Guid.NewGuid();
            this.AssetManager.SetMapping(spriteSheetId, PianoSpriteSheetName);
            var whiteKeyUnpressed = new Sprite(spriteSheetId, WhiteUnpressedKeySpriteLocation, PianoKeySpriteSize);
            var whiteKeyPressed = new Sprite(spriteSheetId, WhitePressedKeySpriteLocation, PianoKeySpriteSize);
            var blackKeyUnpressed = new Sprite(spriteSheetId, BlackUnpressedKeySpriteLocation, PianoKeySpriteSize);
            var blackKeyPressed = new Sprite(spriteSheetId, BlackPressedKeySpriteLocation, PianoKeySpriteSize);
            var songPlayer = new LiveSongPlayer(new Song());
            this.CurrentScene.AddChild(new PianoComponent(songPlayer, whiteKeyUnpressed, whiteKeyPressed, blackKeyUnpressed, blackKeyPressed));
            this._isLoaded = true;
        }
    }
}