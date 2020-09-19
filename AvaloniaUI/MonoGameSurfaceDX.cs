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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A surface for a MonoGame <see cref="Gra" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Control" />
    public sealed class MonoGameSurfaceDX : Control, IDisposable {
        private static readonly MonoGameGraphicsDeviceService _graphicsDeviceService = new MonoGameGraphicsDeviceService();
        private readonly GameTime _gameTime = new GameTime();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private WriteableBitmap _bitmap;
        private Image _image;
        private int _instanceCount;
        private bool _isFirstLoad = true;
        private bool _isInitialized;
        private IMonoGameViewModel _viewModel;

        public MonoGameSurfaceDX() {
            this._instanceCount++;
            this.Focusable = true;
        }

        ~MonoGameSurfaceDX() {
            this.Dispose(false);
        }

        public bool FocusOnMouseOver { get; set; } = true;

        public bool IsDisposed { get; private set; }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void Render(DrawingContext context) {
            if (this._isFirstLoad) {
                _graphicsDeviceService.Initialize(this.GetVisualRoot() as Window);
                this._viewModel?.Initialize();
                this._viewModel?.LoadContent();
                this._isFirstLoad = false;
            }

            this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
            this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
            this._stopwatch.Restart();

            if (this.CanBeginDraw()) {
                try {
                    if (this._bitmap == null) {
                        this._bitmap = new WriteableBitmap(
                            new PixelSize((int)this.Width, (int)this.Height),
                            new Vector(96d, 96d),
                            PixelFormat.Rgba8888);

                        this._viewModel?.SizeChanged(this._bitmap.Size);
                    }

                    using (var bitmapLock = this._bitmap.Lock()) {
                        this.SetViewport();

                        this._viewModel?.Update(this._gameTime);
                        this._viewModel?.Draw(this._gameTime);

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
                context.DrawImage(this._bitmap, 100d, sourceRect, destRect, interpolationMode);
            }

            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.Background);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);
            this.Start();
        }

        protected override void OnDataContextChanged(EventArgs e) {
            base.OnDataContextChanged(e);
            this._viewModel = this.DataContext as IMonoGameViewModel;

            if (this._viewModel != null) {
                this._viewModel.GraphicsDeviceService = _graphicsDeviceService;
                Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);
            this._viewModel?.UnloadContent();
        }

        protected override void OnGotFocus(GotFocusEventArgs e) {
            this._viewModel?.OnActivated(this, EventArgs.Empty);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e) {
            this._viewModel?.OnDeactivated(this, EventArgs.Empty);
            base.OnLostFocus(e);
        }

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

            if (deviceNeedsReset ||
                this._viewModel.GraphicsDeviceService.GraphicsDevice.PresentationParameters.BackBufferWidth != this.Bounds.Width ||
                this._viewModel.GraphicsDeviceService.GraphicsDevice.PresentationParameters.BackBufferHeight != this.Bounds.Height) {
                this._viewModel.GraphicsDeviceService.ResetDevice((int)this.Bounds.Width, (int)this.Bounds.Height);
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

            this._image = new Image { Source = _bitmap, Stretch = Stretch.None };
            this._image.Focusable = true;

            var window = this.GetVisualRoot() as Window;

            window.Closing += (sender, args) => this._viewModel?.OnExiting(this, EventArgs.Empty);

            this.VisualChildren.Add(this._image);
            this._stopwatch.Start();
            this._isInitialized = true;
        }
    }
}