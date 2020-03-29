namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public sealed class PianoComponent : BaseComponent, IUpdateableComponent {
        private readonly Camera _camera;

        private readonly FilterSortCollection<IClickablePianoComponent> _clickables = new FilterSortCollection<IClickablePianoComponent>(
            c => c.IsClickable,
            (c, handler) => c.ClickabilityChanged += handler,
            (c, handler) => c.ClickabilityChanged -= handler,
            (c1, c2) => Comparer<int>.Default.Compare(c1.Priority, c2.Priority),
            (c, handler) => { },
            (c, handler) => { });

        private readonly NotifyCollectionChangedEventHandler _componentChildrenChangedHandler;
        private readonly Instrument _instrument = new Instrument(); // TODO: dynamic
        private readonly Song _song = new Song(); // TODO: dynamic

        private IClickablePianoComponent _currentClickable;

        public PianoComponent() : base() {
            this._camera = this.AddChild<Camera>();
            this._componentChildrenChangedHandler = new NotifyCollectionChangedEventHandler(this.Component_ChildrenChanged);
        }

        public void ResetCamera() {
            this._camera.ViewHeight = SceneHelper.SceneHeight;
            var viewWidth = this._camera.GetViewWidth();
            this._camera.LocalPosition = new Vector2((0.5f * viewWidth) - 1f, this._camera.ViewHeight * 0.5f);
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
                    if (clickable.TryClick(mouseWorldPosition, this._song, this._instrument)) {
                        this._currentClickable = clickable;
                        break;
                    }
                }
            }
        }

        public void Update(FrameTime frameTime) => this.Update(frameTime, Mouse.GetState(), Keyboard.GetState());

        protected override void Initialize() {
            var pitches = Enum.GetValues(typeof(Pitch)).Cast<Pitch>().OrderBy(x => x).ToList();
            var notes = Enum.GetValues(typeof(Note)).Cast<Note>().OrderBy(x => x).ToList();

            foreach (var pitch in pitches) {
                foreach (var note in notes) {
                    this.AddChild(new PianoKeyComponent(note, pitch));
                }
            }

            this.ResetCamera();

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