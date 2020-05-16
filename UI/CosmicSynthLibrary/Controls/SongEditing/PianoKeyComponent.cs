namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;

    public sealed class PianoKeyComponent : SimpleBodyComponent, IClickablePianoRollComponent {
        private static readonly Point BlackKeySpriteLocation = new Point(0, 0);
        private static readonly Point PianoKeySpriteSize = new Point(32, 16);
        private static readonly Point WhiteKeySpriteLocation = new Point(0, 16);
        private readonly IPianoRoll _pianoRoll;
        private SpriteRenderComponent _spriteRenderer;

        public PianoKeyComponent(Note note, Pitch pitch, IPianoRoll pianoRoll) : base() {
            this._pianoRoll = pianoRoll;
            this.LocalPosition = new Vector2(0f, this._pianoRoll.GetRowPosition(note, pitch));
            this.Frequency = new Frequency(note, pitch);
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        public Frequency Frequency { get; }

        public bool IsClickable {
            get {
                return this.IsEnabled;
            }
        }

        public int Priority {
            get {
                return 0;
            }
        }

        public void EndClick() {
            this._pianoRoll.StopNote(this.Frequency);
        }

        public bool TryClick(Vector2 mouseWorldPosition) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                this._pianoRoll.PlayNote(this.Frequency);
                result = true;
            }

            return result;
        }

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
            this._spriteRenderer.Sprite = new Sprite(AssetManager.Instance.GetId(PianoRoll.SpriteSheetPath), this.Frequency.Note.IsNatural() ? WhiteKeySpriteLocation : BlackKeySpriteLocation, PianoKeySpriteSize);
            this._spriteRenderer.OnInitialized += this.SpriteRenderer_OnInitialized;
            this.Collider = new RectangleCollider(2f, 1f);
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