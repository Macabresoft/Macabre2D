namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Threading;
    using Avalonia.VisualTree;
    using Macabresoft.MonoGame.AvaloniaUI.Extensions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A surface for a MonoGame <see cref="GraphicsDevice" />.
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Control" />
    public sealed class MonoGameSurfaceDX : Border, IDisposable {
        private static readonly MonoGameGraphicsDeviceService _graphicsDeviceService = new MonoGameGraphicsDeviceService();
        private readonly GameTime _gameTime = new GameTime();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private WriteableBitmap _bitmap;
        private Microsoft.Xna.Framework.Color _currentBackground;
        private int _instanceCount;
        private bool _isFirstLoad = true;
        private bool _isInitialized;
        private bool _isResizeProcessing = false;
        private IMonoGameViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameSurfaceDX" /> class.
        /// </summary>
        public MonoGameSurfaceDX() {
            this._instanceCount++;
            this.Focusable = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MonoGameSurfaceDX" /> class.
        /// </summary>
        ~MonoGameSurfaceDX() {
            this.Dispose(false);
        }

        /// <inheritdoc />
        public bool FocusOnMouseOver { get; set; } = true;

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override void Render(DrawingContext context) {
            base.Render(context);

            if (this._viewModel != null) {
                if (this._viewModel.Scene.BackgroundColor != this._currentBackground) {
                    this._currentBackground = this._viewModel.Scene.BackgroundColor;
                    this.Background = this._currentBackground.ToAvaloniaBrush();
                }

                if (this._isFirstLoad) {
                    _graphicsDeviceService.Initialize(this.GetVisualRoot() as Window);
                    this._viewModel.GraphicsDeviceService = _graphicsDeviceService;
                    this._viewModel.Initialize();
                    this._viewModel.LoadContent();
                    this._isFirstLoad = false;
                }
            }

            this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
            this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
            this._stopwatch.Restart();

            if (this._bitmap == null || this._bitmap.Size.Width != this.Bounds.Width || this._bitmap.Size.Height != this.Bounds.Height) {
                this._bitmap = new WriteableBitmap(
                    new PixelSize((int)this.Bounds.Width, (int)this.Bounds.Height),
                    new Vector(96d, 96d),
                    PixelFormat.Rgba8888);

                if (!this._isResizeProcessing) {
                    this._isResizeProcessing = true;
                    Dispatcher.UIThread.Post(() => {
                        this._viewModel?.OnSizeChanged(this._bitmap.Size);
                        this._isResizeProcessing = false;
                    }, DispatcherPriority.ContextIdle);
                }
            }

            if (this.CanBeginDraw()) {
                try {
                    using (var bitmapLock = this._bitmap.Lock()) {
                        this.SetViewport();

                        this._viewModel.Update(this._gameTime);
                        this._viewModel.Draw(this._gameTime);

                        var data = new byte[bitmapLock.RowBytes * bitmapLock.Size.Height];
                        this._viewModel.GraphicsDeviceService.GraphicsDevice.GetBackBufferData(data);
                        Marshal.Copy(data, 0, bitmapLock.Address, data.Length);
                        this._viewModel.GraphicsDeviceService.GraphicsDevice.Flush();
                    }
                }
                finally {
                    this._viewModel.GraphicsDeviceService.GraphicsDevice.SetRenderTarget(null);
                }

                Rect viewPort = new Rect(this.Bounds.Size);
                Rect destRect = viewPort.CenterRect(new Rect(this._bitmap.Size)).Intersect(viewPort);
                Rect sourceRect = new Rect(this._bitmap.Size).CenterRect(new Rect(destRect.Size));

                var interpolationMode = RenderOptions.GetBitmapInterpolationMode(this);
                context.DrawImage(this._bitmap, 1d, sourceRect, destRect, interpolationMode);
            }

            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.Background);
        }

        /// <inheritdoc />
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);
            this.Start();
        }

        /// <inheritdoc />
        protected override void OnDataContextChanged(EventArgs e) {
            base.OnDataContextChanged(e);

            this._viewModel = this.DataContext as IMonoGameViewModel;

            if (this._viewModel != null) {
                this._currentBackground = this._viewModel.Scene.BackgroundColor;
                this.Background = this._currentBackground.ToAvaloniaBrush();
                this._viewModel.GraphicsDeviceService = _graphicsDeviceService;
                Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
            }
        }

        /// <inheritdoc />
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);
            this._viewModel?.UnloadContent();
        }

        /// <inheritdoc />
        protected override void OnGotFocus(GotFocusEventArgs e) {
            this._viewModel?.OnActivated(this, EventArgs.Empty);
            base.OnGotFocus(e);
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e) {
            this._viewModel?.OnDeactivated(this, EventArgs.Empty);
            base.OnLostFocus(e);
        }

        /// <inheritdoc />
        private bool CanBeginDraw() {
            // If we have no graphics device, we must be running in the designer. Make sure the
            // graphics device is big enough, and is not lost.
            return this._viewModel?.GraphicsDeviceService != null && this.Bounds.Width > 0 && this.Bounds.Height > 0 && this.HandleDeviceReset();
        }

        private void Dispose(bool disposing) {
            if (!this.IsDisposed) {
                if (disposing) {
                    this._viewModel?.Dispose();
                    this._instanceCount--;

                    if (this._instanceCount <= 0) {
                        _graphicsDeviceService?.Dispose();
                    }
                }

                this.IsDisposed = true;
            }
        }

        private bool HandleDeviceReset() {
            if (this._viewModel?.GraphicsDeviceService?.GraphicsDevice == null) {
                return false;
            }

            var deviceNeedsReset = false;

            switch (this._viewModel.GraphicsDeviceService.GraphicsDevice.GraphicsDeviceStatus) {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return false;

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;
            }

            if (deviceNeedsReset) {
                this._viewModel.GraphicsDeviceService.ResetDevice((int)this.Bounds.Width, (int)this.Bounds.Height);
                return false;
            }
            else if (this._viewModel.GraphicsDeviceService.GraphicsDevice.PresentationParameters.BackBufferWidth != this.Bounds.Width ||
                this._viewModel.GraphicsDeviceService.GraphicsDevice.PresentationParameters.BackBufferHeight != this.Bounds.Height) {
                return false;
            }

            return true;
        }

        private void SetViewport() {
            // Many GraphicsDeviceControl instances can be sharing the same GraphicsDevice. The
            // device backbuffer will be resized to fit the largest of these controls. But what if
            // we are currently drawing a smaller control? To avoid unwanted stretching, we set the
            // viewport to only use the top left portion of the full backbuffer.
            var width = Math.Max(1, (int)this.Bounds.Width);
            var height = Math.Max(1, (int)this.Bounds.Height);
            this._viewModel.GraphicsDeviceService.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
        }

        private void Start() {
            if (this._isInitialized) {
                return;
            }

            var window = this.GetVisualRoot() as Window;
            window.Closing += (sender, args) => this._viewModel?.OnExiting(this, EventArgs.Empty);
            this._stopwatch.Start();
            this._isInitialized = true;
        }
    }
}