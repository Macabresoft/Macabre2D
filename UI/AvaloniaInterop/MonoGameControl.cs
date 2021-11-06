namespace Macabresoft.Macabre2D.UI.AvaloniaInterop {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Threading;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop.Extensions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A control that renders a MonoGame instance inside of Avalonia.
    /// </summary>
    public sealed class MonoGameControl : Control, IDisposable {
        private readonly Dictionary<StandardCursorType, Cursor> _cursorTypeToCursor = new() {
            { StandardCursorType.None, null }
        };

        private readonly GameTime _gameTime = new();
        private readonly Stopwatch _stopwatch = new();
        private WriteableBitmap _bitmap;
        private bool _isDisposed;
        private bool _isInitialized;
        private bool _isResizeProcessing;
        private MonoGameKeyboard _keyboard;
        private MonoGameMouse _mouse;
        private IMonoGameViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        public MonoGameControl() {
            this.Focusable = true;
        }

        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override void Render(DrawingContext context) {
            if (this.IsEffectivelyVisible && this._viewModel != null) {
                base.Render(context);

                if (this._viewModel.Scene != this._viewModel.Game.Scene) {
                    this._viewModel.ResetScene();
                }

                this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
                this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
                this._stopwatch.Restart();

                if (this.CanBeginDraw()) {
                    using (var bitmapLock = this._bitmap.Lock()) {
                        this._viewModel.RunFrame();

                        var data = new byte[bitmapLock.RowBytes * bitmapLock.Size.Height];
                        this._viewModel.GraphicsDevice.GetBackBufferData(data);
                        Marshal.Copy(data, 0, bitmapLock.Address, data.Length);
                    }

                    context.DrawImage(this._bitmap, new Rect(this._bitmap.Size), new Rect(this.Bounds.Size));
                }
                else if (this._viewModel.Scene is IScene scene) {
                    context.DrawRectangle(scene.BackgroundColor.ToAvaloniaBrush(), null, new Rect(this.Bounds.Size));
                }
                else if (this._viewModel.Game?.Project.Settings is IGameSettings settings) {
                    context.DrawRectangle(settings.FallbackBackgroundColor.ToAvaloniaBrush(), null, new Rect(this.Bounds.Size));
                }
            }

            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.Render);
        }

        /// <inheritdoc />
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);
            this.Start();
        }

        /// <inheritdoc />
        protected override void OnDataContextChanged(EventArgs e) {
            base.OnDataContextChanged(e);

            if (this._viewModel != null) {
                this._viewModel.PropertyChanged -= this.ViewModel_PropertyChanged;
            }

            this._viewModel = this.DataContext as IMonoGameViewModel;

            if (this._viewModel != null) {
                this._viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
                this.InitializeViewModel();
                Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual, DispatcherPriority.Background);
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

        /// <inheritdoc />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.Bounds)) {
                this.HandleResize();
            }
        }


        private bool CanBeginDraw() {
            // If we have no graphics device, we must be running in the designer. Make sure the
            // graphics device is big enough, and is not lost.
            return !this._isResizeProcessing && this._viewModel?.GraphicsDevice != null && this.Bounds.Width > 0 && this.Bounds.Height > 0 && this.HandleDeviceReset();
        }

        private void Dispose(bool disposing) {
            if (!this._isDisposed) {
                if (disposing) {
                    this._viewModel?.Dispose();
                }

                this._isDisposed = true;
            }
        }

        private bool HandleDeviceReset() {
            if (this._viewModel?.GraphicsDevice == null || this._viewModel.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Lost) {
                return false;
            }

            if (this._viewModel.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset ||
                this._viewModel.GraphicsDevice.PresentationParameters.BackBufferWidth != this._bitmap.PixelSize.Width ||
                this._viewModel.GraphicsDevice.PresentationParameters.BackBufferHeight != this._bitmap.PixelSize.Height) {
                this._viewModel.ResetDevice(this._bitmap.PixelSize.Width, this._bitmap.PixelSize.Height);
                return false;
            }

            return true;
        }

        private void HandleResize() {
            if (this.ShouldPerformResize()) {
                this._isResizeProcessing = true;
                this._bitmap?.Dispose();

                this._bitmap = new WriteableBitmap(
                    new PixelSize((int)Math.Ceiling(this.Bounds.Width), (int)Math.Ceiling(this.Bounds.Height)),
                    new Vector(96d, 96d),
                    PixelFormat.Rgba8888,
                    AlphaFormat.Opaque);

                this.SetViewport();

                Dispatcher.UIThread.Post(() => {
                    this._viewModel?.OnSizeChanged(this._bitmap.Size);
                    this._isResizeProcessing = false;
                }, DispatcherPriority.ContextIdle);
            }
        }

        private void InitializeViewModel(Window window) {
            if (this._viewModel != null) {
                this._viewModel.RunFrame();
                this._viewModel.Initialize(window, this.Bounds.Size, this._mouse, this._keyboard);
            }
        }

        private void InitializeViewModel() {
            if (this.GetVisualRoot() is Window window) {
                this.InitializeViewModel(window);
            }
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
                    Math.Abs(this._bitmap.PixelSize.Width - Math.Ceiling(this.Bounds.Width)) > 0.01f ||
                    Math.Abs(this._bitmap.PixelSize.Height - Math.Ceiling(this.Bounds.Height)) > 0.01f);
        }

        private void Start() {
            if (this._isInitialized) {
                return;
            }

            this._mouse = new MonoGameMouse(this);
            this._keyboard = new MonoGameKeyboard(this);

            if (this.GetVisualRoot() is Window window) {
                window.Closing += (_, _) => this._viewModel?.OnExiting(this, EventArgs.Empty);
                this.InitializeViewModel(window);
            }

            this.HandleResize();
            this._stopwatch.Start();
            this._isInitialized = true;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IMonoGameViewModel.CursorType)) {
                if (!this._cursorTypeToCursor.TryGetValue(this._viewModel.CursorType, out var cursor)) {
                    cursor = new Cursor(this._viewModel.CursorType);
                    this._cursorTypeToCursor.Add(this._viewModel.CursorType, cursor);
                }

                this.Cursor = cursor;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        ~MonoGameControl() {
            this.Dispose(false);
        }
    }
}