// This code was generated from the template provided from craftworkgames MonoGame.WpfCore: github.com/craftworkgames/MonoGame.WpfCore

namespace Macabresoft.MonoGame.AvaloniaUI {

    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class MonoGameGraphicsDeviceService : IGraphicsDeviceService, IDisposable {
        private readonly DefaultGame _game = new DefaultGame();
        private bool _isDisposed = false;

        public MonoGameGraphicsDeviceService() {
        }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice => this._game.GraphicsDevice;

        public void Dispose() {
            this.Dispose(true);
        }

        public void Initialize() {
            this._game.RunOneFrame();
            this.GraphicsDevice.Reset();
            this.DeviceCreated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets the graphics device to whichever is bigger out of the specified resolution or its
        /// current size. This behavior means the device will demand-grow to the largest of all its
        /// GraphicsDeviceControl clients.
        /// </summary>
        public void ResetDevice(int width, int height) {
            var newWidth = Math.Max(1, width);
            var newHeight = Math.Max(1, height);

            if (newWidth != this.GraphicsDevice.PresentationParameters.BackBufferWidth || newHeight != this.GraphicsDevice.PresentationParameters.BackBufferHeight) {
                DeviceResetting?.Invoke(this, EventArgs.Empty);

                this.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
                this.GraphicsDevice.PresentationParameters.BackBufferWidth = newWidth;
                this.GraphicsDevice.PresentationParameters.BackBufferHeight = newHeight;
                this.DeviceReset?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                if (disposing) {
                    DeviceDisposing?.Invoke(this, EventArgs.Empty);
                    this._game.Dispose();
                }

                this._isDisposed = true;
            }
        }
    }
}