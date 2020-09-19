namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia;
    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.ComponentModel;

    public interface IMonoGameViewModel : IDisposable, INotifyPropertyChanged {
        MonoGameGraphicsDeviceService GraphicsDeviceService { get; set; }

        void Draw(GameTime gameTime);

        void Initialize();

        void LoadContent();

        void OnActivated(object sender, EventArgs args);

        void OnDeactivated(object sender, EventArgs args);

        void OnExiting(object sender, EventArgs args);

        void SizeChanged(Size newSize);

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

        public MonoGameGraphicsDeviceService GraphicsDeviceService { get; set; }

        protected MonoGameServiceProvider Services { get; private set; }

        public void Dispose() {
            this.Dispose(true);
        }

        public virtual void Draw(GameTime gameTime) {
        }

        public virtual void Initialize() {
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

        public virtual void SizeChanged(Size newSize) {
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