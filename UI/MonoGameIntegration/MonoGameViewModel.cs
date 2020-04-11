namespace Macabre2D.UI.MonoGameIntegration {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.ComponentModel;
    using System.Windows;

    public interface IMonoGameViewModel : IDisposable, INotifyPropertyChanged {
        IGraphicsDeviceService GraphicsDeviceService { get; set; }

        MonoGameKeyboard Keyboard { get; }

        MonoGameMouse Mouse { get; }

        void Draw(GameTime gameTime);

        void Initialize(MonoGameKeyboard keyboard, MonoGameMouse mouse);

        void LoadContent();

        void OnActivated(object sender, EventArgs args);

        void OnDeactivated(object sender, EventArgs args);

        void OnExiting(object sender, EventArgs args);

        void SizeChanged(object sender, SizeChangedEventArgs args);

        void UnloadContent();

        void Update(GameTime gameTime);
    }

    public class MonoGameViewModel : NotifyPropertyChanged, IMonoGameViewModel {
        private bool _isDisposed = false;

        public MonoGameViewModel() {
        }

        public ContentManager Content { get; set; }

        public GraphicsDevice GraphicsDevice {
            get {
                return this.GraphicsDeviceService?.GraphicsDevice;
            }
        }

        public IGraphicsDeviceService GraphicsDeviceService { get; set; }

        public MonoGameKeyboard Keyboard { get; private set; }

        public MonoGameMouse Mouse { get; private set; }

        protected MonoGameServiceProvider Services { get; private set; }

        public void Dispose() {
            this.Dispose(true);
        }

        public virtual void Draw(GameTime gameTime) {
        }

        public virtual void Initialize(MonoGameKeyboard keyboard, MonoGameMouse mouse) {
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.Services = new MonoGameServiceProvider();
            this.Services.AddService(this.GraphicsDeviceService);
            this.Content = new ContentManager(this.Services) { RootDirectory = "Content" };
        }

        public virtual void LoadContent() {
        }

        public virtual void OnActivated(object sender, EventArgs args) {
        }

        public virtual void OnDeactivated(object sender, EventArgs args) {
        }

        public virtual void OnExiting(object sender, EventArgs args) {
        }

        public virtual void SizeChanged(object sender, SizeChangedEventArgs args) {
        }

        public virtual void UnloadContent() {
        }

        public virtual void Update(GameTime gameTime) {
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                if (disposing) {
                    this.Content?.Dispose();
                }

                this._isDisposed = true;
            }
        }
    }
}