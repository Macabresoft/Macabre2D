namespace Macabresoft.Macabre2D.AvaloniaUI {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Threading;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.AvaloniaUI.Extensions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A control that renders a MonoGame instance inside of Avalonia.
    /// </summary>
    public sealed class MonoGameControl : Border, IDisposable {
        private readonly GameTime _gameTime = new GameTime();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private WriteableBitmap _bitmap;
        private Microsoft.Xna.Framework.Color _currentBackground;
        private bool _isFirstLoad = true;
        private bool _isInitialized;
        private bool _isResizeProcessing = false;
        private MonoGameKeyboard _keyboard;
        private MonoGameMouse _mouse;
        private IMonoGameViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        public MonoGameControl() {
            this.Focusable = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        ~MonoGameControl() {
            this.Dispose(false);
        }

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
                if (this._viewModel.Game.Scene.BackgroundColor != this._currentBackground) {
                    this._currentBackground = this._viewModel.Game.Scene.BackgroundColor;
                    this.Background = this._currentBackground.ToAvaloniaBrush();
                }

                if (this._isFirstLoad) {
                    if (this.GetVisualRoot() is Window window) {
                        this._viewModel.Initialize(window, this.Bounds.Size, this._mouse, this._keyboard);
                        this._isFirstLoad = false;
                    }
                }
            }

            this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
            this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
            this._stopwatch.Restart();

            if (this.ShouldPerformResize()) {
                this._bitmap = new WriteableBitmap(
                    new PixelSize((int)Math.Ceiling(this.Bounds.Width), (int)Math.Ceiling(this.Bounds.Height)),
                    new Vector(96d, 96d),
                    PixelFormat.Rgba8888,
                    AlphaFormat.Opaque);

                this.SetViewport();

                this._isResizeProcessing = true;
                Dispatcher.UIThread.Post(() => {
                    this._viewModel?.OnSizeChanged(this._bitmap.Size);
                    this._isResizeProcessing = false;
                }, DispatcherPriority.ContextIdle);
            }

            if (this.CanBeginDraw()) {
                using (var bitmapLock = this._bitmap.Lock()) {
                    this._viewModel.RunFrame();

                    var data = new byte[bitmapLock.RowBytes * bitmapLock.Size.Height];
                    this._viewModel.GraphicsDevice.GetBackBufferData(data);
                    Marshal.Copy(data, 0, bitmapLock.Address, data.Length);
                }

                Rect viewPort = new Rect(this.Bounds.Size);
                var interpolationMode = RenderOptions.GetBitmapInterpolationMode(this);
                context.DrawImage(this._bitmap, viewPort, viewPort, interpolationMode);
            }

            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.MaxValue);
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
                this._currentBackground = this._viewModel.Game.Scene.BackgroundColor;
                this.Background = this._currentBackground.ToAvaloniaBrush();
                Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
            }
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

        private bool CanBeginDraw() {
            // If we have no graphics device, we must be running in the designer. Make sure the
            // graphics device is big enough, and is not lost.
            return !this._isResizeProcessing && this._viewModel?.GraphicsDevice != null && this.Bounds.Width > 0 && this.Bounds.Height > 0 && this.HandleDeviceReset();
        }

        private void Dispose(bool disposing) {
            if (!this.IsDisposed) {
                if (disposing) {
                    this._viewModel?.Dispose();
                }

                this.IsDisposed = true;
            }
        }

        private bool HandleDeviceReset() {
            if (this._viewModel?.GraphicsDevice == null) {
                return false;
            }

            var deviceNeedsReset = false;
            switch (this._viewModel.GraphicsDevice.GraphicsDeviceStatus) {
                case GraphicsDeviceStatus.Lost:
                    return false;

                case GraphicsDeviceStatus.NotReset:
                    deviceNeedsReset = true;
                    break;
            }

            if (deviceNeedsReset ||
                this._viewModel.GraphicsDevice.PresentationParameters.BackBufferWidth != this._bitmap.PixelSize.Width ||
                this._viewModel.GraphicsDevice.PresentationParameters.BackBufferHeight != this._bitmap.PixelSize.Height) {
                this._viewModel.ResetDevice(this._bitmap.PixelSize.Width, this._bitmap.PixelSize.Height);
                return false;
            }

            return true;
        }

        private void SetViewport() {
            if (this._viewModel.GraphicsDevice != null) {
                var width = Math.Max(1, (int)this.Bounds.Width);
                var height = Math.Max(1, (int)this.Bounds.Height);
                this._viewModel.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
            }
        }

        private bool ShouldPerformResize() {
            return !this._isResizeProcessing &&
                this.Bounds.Width > 0 &&
                this.Bounds.Height > 0 &&
                (this._bitmap == null ||
                this._bitmap.PixelSize.Width != Math.Ceiling(this.Bounds.Width) ||
                this._bitmap.PixelSize.Height != Math.Ceiling(this.Bounds.Height));
        }

        private void Start() {
            if (this._isInitialized) {
                return;
            }

            this._mouse = new MonoGameMouse(this);
            this._keyboard = new MonoGameKeyboard(this);
            var window = this.GetVisualRoot() as Window;
            window.Closing += (sender, args) => this._viewModel?.OnExiting(this, EventArgs.Empty);
            this._stopwatch.Start();
            this._isInitialized = true;
        }
    }
}