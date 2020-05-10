namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.MonoGameIntegration;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class PianoComponent : BaseComponent, IUpdateableComponent {

        private readonly FilterSortCollection<IClickablePianoRollComponent> _clickables = new FilterSortCollection<IClickablePianoRollComponent>(
            c => c.IsClickable,
            nameof(IClickablePianoRollComponent.IsClickable),
            (c1, c2) => Comparer<int>.Default.Compare(c1.Priority, c2.Priority),
            nameof(IClickablePianoRollComponent.Priority));

        private readonly NotifyCollectionChangedEventHandler _componentChildrenChangedHandler;
        private readonly IPianoRoll _pianoRoll;
        private Camera _camera;

        private IClickablePianoRollComponent _currentClickable;

        public PianoComponent(IPianoRoll pianoRoll) : base() {
            this._pianoRoll = pianoRoll;
            this._componentChildrenChangedHandler = new NotifyCollectionChangedEventHandler(this.Component_ChildrenChanged);
        }

        public void Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState) {
            var mouseWorldPosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);
            var isHandled = false;
            if (this._currentClickable != null) {
                if (mouseState.LeftButton == ButtonState.Pressed) {
                    isHandled = this._currentClickable.TryHoldClick(mouseWorldPosition);
                }
                else {
                    this._currentClickable.EndClick();
                    this._currentClickable = null;
                    isHandled = true;
                }
            }

            if (!isHandled && mouseState.LeftButton == ButtonState.Pressed) {
                this._clickables.RebuildCache();
                foreach (var clickable in this._clickables) {
                    if (clickable.TryClick(mouseWorldPosition)) {
                        this._currentClickable = clickable;
                        break;
                    }
                }
            }
        }

        public void Update(FrameTime frameTime) => this.Update(frameTime, MonoGameMouse.Instance.GetState(), MonoGameKeyboard.Instance.GetState());

        protected override void Initialize() {
            var pitches = Enum.GetValues(typeof(Pitch)).Cast<Pitch>().OrderBy(x => x).ToList();
            var notes = Enum.GetValues(typeof(Note)).Cast<Note>().OrderBy(x => x).ToList();

            foreach (var pitch in pitches) {
                foreach (var note in notes) {
                    this.AddChild(new PianoKeyComponent(note, pitch, this._pianoRoll));
                }
            }

            this._camera = this.Scene.FindComponentOfType<Camera>();

            foreach (var child in this.GetChildrenOfType<IClickablePianoRollComponent>()) {
                this._clickables.Add(child);
            }

            this.SubscribeToChildrenChanged(this._componentChildrenChangedHandler);
        }

        private void Component_ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var component in e.NewItems.OfType<IClickablePianoRollComponent>()) {
                    this._clickables.Add(component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var component in e.OldItems.OfType<IClickablePianoRollComponent>()) {
                    this._clickables.Remove(component);
                }
            }
        }
    }
}