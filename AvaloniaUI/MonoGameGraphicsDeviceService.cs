// This code was generated from the template provided from craftworkgames MonoGame.WpfCore: github.com/craftworkgames/MonoGame.WpfCore

namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia.Controls;
    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class MonoGameGraphicsDeviceService : IGraphicsDeviceService, IDisposable {
        private readonly DefaultGame _game = new DefaultGame();

        private readonly PresentationParameters _presentationParameters = new PresentationParameters() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

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

        public void Initialize(Window window) {
            this._game.RunOneFrame();
            this._presentationParameters.DeviceWindowHandle = window.PlatformImpl.Handle.Handle;
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
                this.DeviceResetting?.Invoke(this, EventArgs.Empty);
                this.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
                this._presentationParameters.BackBufferWidth = Math.Max(width, 1);
                this._presentationParameters.BackBufferHeight = Math.Max(height, 1);
                this.GraphicsDevice.Reset(this._presentationParameters);
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