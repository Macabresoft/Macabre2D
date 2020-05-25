namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class PianoComponent : BaseComponent, IUpdateableComponent {
        private readonly Sprite _blackKeyPressed;
        private readonly Sprite _blackKeyUnpressed;

        private readonly FilterSortCollection<IClickablePianoComponent> _clickables = new FilterSortCollection<IClickablePianoComponent>(
            c => c.IsClickable,
            nameof(IClickablePianoComponent.IsClickable),
            (c1, c2) => Comparer<int>.Default.Compare(c1.Priority, c2.Priority),
            nameof(IClickablePianoComponent.Priority));

        private readonly NotifyCollectionChangedEventHandler _componentChildrenChangedHandler;
        private readonly LiveSongPlayer _songPlayer;
        private readonly Sprite _whiteKeyPressed;
        private readonly Sprite _whiteKeyUnpressed;
        private Camera _camera;
        private IClickablePianoComponent _currentClickable;

        /// <summary>
        /// Initializes a new instance of the <see cref="PianoComponent"/> class.
        /// </summary>
        /// <param name="songPlayer">The song player.</param>
        /// <param name="whiteKeyUnpressed">The white key unpressed sprite.</param>
        /// <param name="whiteKeyPressed">The white key pressed sprite.</param>
        /// <param name="blackKeyUnpressed">The black key unpressed sprite.</param>
        /// <param name="blackKeyPressed">The black key pressed sprite.</param>
        public PianoComponent(LiveSongPlayer songPlayer, Sprite whiteKeyUnpressed, Sprite whiteKeyPressed, Sprite blackKeyUnpressed, Sprite blackKeyPressed) : base() {
            this._songPlayer = songPlayer ?? throw new ArgumentNullException(nameof(songPlayer));
            this._whiteKeyUnpressed = whiteKeyUnpressed ?? throw new ArgumentNullException(nameof(whiteKeyUnpressed));
            this._whiteKeyPressed = whiteKeyPressed ?? throw new ArgumentNullException(nameof(whiteKeyPressed));
            this._blackKeyUnpressed = blackKeyUnpressed ?? throw new ArgumentNullException(nameof(blackKeyUnpressed));
            this._blackKeyPressed = blackKeyPressed ?? throw new ArgumentNullException(nameof(blackKeyPressed));
            this._componentChildrenChangedHandler = new NotifyCollectionChangedEventHandler(this.Component_ChildrenChanged);
        }

        /// inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            var mouseWorldPosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
            var isHandled = false;
            if (this._currentClickable != null) {
                if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                    isHandled = this._currentClickable.TryHoldClick(mouseWorldPosition);
                }
                else {
                    this._currentClickable.EndClick();
                    this._currentClickable = null;
                    isHandled = true;
                }
            }

            if (!isHandled && inputState.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                this._clickables.RebuildCache();
                foreach (var clickable in this._clickables) {
                    if (clickable.TryClick(mouseWorldPosition)) {
                        this._currentClickable = clickable;
                        break;
                    }
                }
            }

            this._songPlayer.Buffer(0.8f);
        }

        protected override void Initialize() {
            var pitches = Enum.GetValues(typeof(Pitch)).Cast<Pitch>().OrderBy(x => x).ToList();
            var notes = Enum.GetValues(typeof(Note)).Cast<byte>().Distinct().Cast<Note>().OrderBy(x => x).ToList();

            foreach (var pitch in pitches) {
                foreach (var note in notes) {
                    if (note.IsNatural()) {
                        this.AddChild(new PianoKeyComponent(note, pitch, this._songPlayer, this._whiteKeyPressed, this._whiteKeyUnpressed));
                    }
                    else {
                        this.AddChild(new PianoKeyComponent(note, pitch, this._songPlayer, this._blackKeyPressed, this._blackKeyUnpressed));
                    }
                }
            }

            this._camera = this.Scene.FindComponentOfType<Camera>();

            foreach (var child in this.GetChildrenOfType<IClickablePianoComponent>()) {
                this._clickables.Add(child);
            }

            this.SubscribeToChildrenChanged(this._componentChildrenChangedHandler);
        }

        private void Component_ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var component in e.NewItems.OfType<IClickablePianoComponent>()) {
                    this._clickables.Add(component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var component in e.OldItems.OfType<IClickablePianoComponent>()) {
                    this._clickables.Remove(component);
                }
            }
        }
    }
}