namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// A piano key inside of a <see cref="PianoComponent"/>.
    /// </summary>
    public sealed class PianoKeyComponent : SimpleBodyComponent, IClickablePianoComponent {
        private static readonly Point BlackPressedKeySpriteLocation = new Point(0, 32);
        private static readonly Point BlackUnpressedKeySpriteLocation = new Point(0, 0);
        private static readonly Point PianoKeySpriteSize = new Point(32, 16);
        private static readonly Point WhitePressedKeySpriteLocation = new Point(0, 48);
        private static readonly Point WhiteUnpressedKeySpriteLocation = new Point(0, 16);
        private readonly LiveSongPlayer _songPlayer;
        private Sprite _pressedSprite;
        private SpriteRenderComponent _spriteRenderer;
        private Sprite _unpressedSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="PianoKeyComponent"/> class.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="pitch">The pitch.</param>
        /// <param name="songPlayer">The song player.</param>
        public PianoKeyComponent(Note note, Pitch pitch, LiveSongPlayer songPlayer) : base() {
            this._songPlayer = songPlayer;
            this.LocalPosition = new Vector2(0f, this.GetRowPosition(note, pitch));
            this.Frequency = new Frequency(note, pitch);
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public Frequency Frequency { get; }

        /// <inheritdoc/>
        public bool IsClickable {
            get {
                return this.IsEnabled;
            }
        }

        /// <inheritdoc/>
        public int Priority {
            get {
                return 0;
            }
        }

        /// <inheritdoc/>
        public void EndClick() {
            this._songPlayer.StopNote(this.Frequency);
            this._spriteRenderer.Sprite = this._unpressedSprite;
        }

        /// <inheritdoc/>
        public bool TryClick(Vector2 mouseWorldPosition) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                this._songPlayer.PlayNote(this.Frequency);
                result = true;
                this._spriteRenderer.Sprite = this._pressedSprite;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryHoldClick(Vector2 mouseWorldPosition) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                result = true;
            }
            else {
                this.EndClick();
            }

            return result;
        }

        protected override void Initialize() {
            base.Initialize();

            this._spriteRenderer = this.AddChild<SpriteRenderComponent>();
            this._spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.BottomLeft;
            var spriteSheetPath = AssetManager.Instance.GetId(LiveSongPlayer.SpriteSheetPath);
            var isNatural = this.Frequency.Note.IsNatural();
            this._unpressedSprite = new Sprite(spriteSheetPath, isNatural ? WhiteUnpressedKeySpriteLocation : BlackUnpressedKeySpriteLocation, PianoKeySpriteSize);
            this._pressedSprite = new Sprite(spriteSheetPath, isNatural ? WhitePressedKeySpriteLocation : BlackPressedKeySpriteLocation, PianoKeySpriteSize);
            this._spriteRenderer.Sprite = this._unpressedSprite;
            this._spriteRenderer.OnInitialized += this.SpriteRenderer_OnInitialized;
            this.Collider = new RectangleCollider(2f, 1f);
        }

        private static float GetPitchMultiplier(Pitch pitch) {
            var result = 0f;
            if (pitch == Pitch.Normal) {
                result = 1f;
            }
            else if (pitch == Pitch.High) {
                result = 2f;
            }

            return result;
        }

        private float GetRowPosition(Note note, Pitch pitch) {
            return 12f * GetPitchMultiplier(pitch) + (byte)note;
        }

        private void Self_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.IsEnabled)) {
                this.RaisePropertyChanged(nameof(this.IsClickable));
            }
        }

        private void SpriteRenderer_OnInitialized(object sender, EventArgs e) {
            this.Collider.Offset = 0.5f * this._spriteRenderer.RenderSettings.Size * GameSettings.Instance.InversePixelsPerUnit;
        }
    }
}